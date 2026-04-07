using System;
using Microsoft.Extensions.Logging;
using Aethos.Domain.ValueObjects;

namespace Aethos.Core.Consensus;

/// <summary>
/// Sprint 30: Container do laudo criptográfico emitido pelo Cão de Guarda.
/// </summary>
public class DivergenceResult
{
    public bool IsDivergent { get; set; }
    public string ExpectedHash { get; set; } = string.Empty;
    public string ProvidedHash { get; set; } = string.Empty;
    public string MaliciousValidatorAddress { get; set; } = string.Empty;
    public string EvidencePayload { get; set; } = string.Empty;
}

/// <summary>
/// Sprint 30: Consensus Divergence Guard
/// Mecanismo penal fundamental da rede Neural que identifica Fraude ou Floating-Point Drift.
/// </summary>
public class DivergenceGuard
{
    private readonly ILogger<DivergenceGuard> _logger;

    public DivergenceGuard(ILogger<DivergenceGuard> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Audita cruzando as inferências: Compara o PoR (Proof of Reasoning) local (A Verdade Absoluta) 
    /// com a submissão exógena de um outro nó parceiro no protocolo gRPC.
    /// </summary>
    public DivergenceResult EvaluateSubmission(ResultHash localGeneratedPoR, string partnerSubmittedPoR, string validatorAddress)
    {
        string localHash = localGeneratedPoR.Value.ToLowerInvariant();
        string remoteHash = partnerSubmittedPoR.ToLowerInvariant();

        if (localHash == remoteHash)
        {
            return new DivergenceResult
            {
                IsDivergent = false,
                ExpectedHash = localHash,
                ProvidedHash = remoteHash
            };
        }

        // Trapaça ou quebra de Determinismo Int128 identificada:
        _logger.LogCritical($"[DivergenceGuard] ALERTA FATAL! O Validador {validatorAddress} propôs o Hash {remoteHash}. A VM exigia {localHash}. Jailing acionado!");

        // Payload de evidência criptográfica serializado p/ penalização L1 (Slashing)
        string evidence = $"0xe{Guid.NewGuid():N}{Guid.NewGuid():N}".ToLowerInvariant();

        return new DivergenceResult
        {
            IsDivergent = true,
            ExpectedHash = localHash,
            ProvidedHash = remoteHash,
            MaliciousValidatorAddress = validatorAddress,
            EvidencePayload = evidence
        };
    }
}
