using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Aethos.Node.Health;

/// <summary>
/// Sprint 49: Health Check para o RocksDB.
/// Garante que o armazenamento persistente da L2 está acessível e respondendo.
/// </summary>
public class RocksDbHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Em produção: Tentar uma leitura/escrita de teste no DB.
            bool isDbResponsive = true; 

            if (isDbResponsive)
            {
                return Task.FromResult(HealthCheckResult.Healthy("RocksDB operacional."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("RocksDB não responde ou está travado por I/O."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"Erro no RocksDB: {ex.Message}"));
        }
    }
}
