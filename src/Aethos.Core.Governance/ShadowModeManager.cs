using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Aethos.Math.FixedPoint;

namespace Aethos.Core.Governance;

/// <summary>
/// Sprint 37: Shadow Mode Manager.
/// Permite o deploy de novas versões dos pesos LSTM em modo "A/B" ou observacional.
/// As execuções ocorrem em paralelo, mas não alteram o State Root da EVM.
/// </summary>
public class ShadowModeManager
{
    private readonly ILogger<ShadowModeManager> _logger;
    private readonly ConcurrentDictionary<string, string> _activeShadowModels;

    public ShadowModeManager(ILogger<ShadowModeManager> logger)
    {
        _logger = logger;
        _activeShadowModels = new ConcurrentDictionary<string, string>();
    }

    /// <summary>
    /// Registra um novo modelo experimental para rodar em "Shadow" sobre transações reais.
    /// </summary>
    public void RegisterShadowModel(string modelId, string version)
    {
        _activeShadowModels.TryAdd(modelId, version);
        _logger.LogInformation($"[SHADOW MODE] Modelo experimental {modelId} (v{version}) registrado para observação passiva.");
    }

    /// <summary>
    /// Simula a execução da inferência sem commit on-chain.
    /// </summary>
    public void ProcessShadowInference(string modelId, FixedPointInt128 result)
    {
        if (_activeShadowModels.ContainsKey(modelId))
        {
            // Log determinístico para comparação posterior (Benchmarking de acurácia L2)
            _logger.LogInformation($"[SHADOW MODE] [AUDIT] Modelo {modelId} inferiu: {result.ToDouble()}. (Sem efeito colateral no Ledger)");
        }
    }
}
