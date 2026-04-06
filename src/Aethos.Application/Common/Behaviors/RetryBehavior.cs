using MediatR;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aethos.Application.Common.Behaviors;

/// <summary>
/// Sprint 41: Retry Behavior (Polly).
/// Interceptor do MediatR que aplica políticas de resiliência caso o I/O da Blockchain
/// sofra timeouts ou variações de latência.
/// </summary>
public class RetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<RetryBehavior<TRequest, TResponse>> _logger;

    public RetryBehavior(ILogger<RetryBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Política de Resiliência: Tentativa de 3 vezes com Backoff Exponencial (2s, 4s, 8s)
        var retryPolicy = Policy
            .Handle<Exception>() // Aqui podemos filtrar para TimeoutException/DbUpdateException futuramente
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(System.Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning($"[RESILIÊNCIA] Falha na execução da transação L2. Tentativa {retryCount} em {timeSpan.TotalSeconds}s. Erro: {exception.Message}");
                    return Task.CompletedTask;
                });

        return await retryPolicy.ExecuteAsync(async () => await next());
    }
}
