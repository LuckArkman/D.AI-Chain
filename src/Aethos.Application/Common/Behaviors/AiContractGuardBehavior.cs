using MediatR;
using Microsoft.Extensions.Logging;
using Aethos.Core.Governance;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aethos.Application.Common.Behaviors;

/// <summary>
/// Sprint 42: AI Guardrail Behavior.
/// Interceptor soberano que interrompe qualquer transação L2 se o 
/// Control Plane de Governança estiver em modo "Global Emergency Pause".
/// </summary>
public class AiContractGuardBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<AiContractGuardBehavior<TRequest, TResponse>> _logger;
    private readonly GovernanceService _governance;

    public AiContractGuardBehavior(ILogger<AiContractGuardBehavior<TRequest, TResponse>> logger, GovernanceService governance)
    {
        _logger = logger;
        _governance = governance;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Sprint 42: Verifica se a governaça barrou as execuções neurais/automáticas da rede.
        if (_governance.IsInferencePaused())
        {
            _logger.LogCritical("[GUARDRAIL] EXECUÇÃO BLOQUEADA! A Governança da Aethos L2 está em Pânico ou Pausa Emergencial.");
            throw new InvalidOperationException("A rede Aethos L2 está operando em modo de Segurança Total (Pausa Global). Transações suspensas.");
        }

        return await next();
    }
}
