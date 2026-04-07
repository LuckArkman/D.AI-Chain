using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace Aethos.Infrastructure.Cache;

/// <summary>
/// Sprint 43: Inference Cache (Redis Memoization).
/// Otimiza a rede L2 evitando inferências LSTM redundantes. 
/// Se o Payload de entrada for idêntico, devolve a decisão selada do Redis.
/// </summary>
public class InferenceCache
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<InferenceCache> _logger;
    private readonly IDatabase _db;

    public InferenceCache(IConnectionMultiplexer redis, ILogger<InferenceCache> logger)
    {
        _redis = redis;
        _logger = logger;
        _db = _redis.GetDatabase();
    }

    /// <summary>
    /// Recupera uma decisão de inferência cacheada via Key-Hash.
    /// </summary>
    public async Task<string?> GetCachedDecisionAsync(string payloadHash)
    {
        var result = await _db.StringGetAsync($"aethos:inf:{payloadHash}");
        if (result.HasValue)
        {
            _logger.LogInformation($"[CACHE] Cache Hit! Devolvendo decisão memoizada para o Hash: {payloadHash}");
            return result.ToString();
        }
        return null;
    }

    /// <summary>
    /// Registra o resultado de uma inferência LSTM no Redis com TTL de 1 hora.
    /// </summary>
    public async Task SetDecisionCacheAsync(string payloadHash, object decision)
    {
        string json = JsonSerializer.Serialize(decision);
        await _db.StringSetAsync($"aethos:inf:{payloadHash}", json, TimeSpan.FromHours(1));
        _logger.LogInformation($"[CACHE] Decisão armazenada no Redis para o Hash: {payloadHash}");
    }

    /// <summary>
    /// Sprint 44: Invalidação massiva de cache via script LUA.
    /// Chamado quando um Patch Update de AI Model (Novos Pesos LSTM) é deferido na rede.
    /// </summary>
    public async Task InvalidateModelCacheAsync()
    {
        _logger.LogWarning("[CACHE] PATCH UPDATE DETECTADO. Invalidando todo cache de inferência da versão anterior...");
        
        // Script LUA para deleção atômica de chaves de inferência L2
        var luaScript = @"
            local keys = redis.call('keys', 'aethos:inf:*')
            for i, k in ipairs(keys) do
                redis.call('del', k)
            end
            return #keys";

        var deletedCount = await _db.ScriptEvaluateAsync(luaScript);
        _logger.LogCritical($"[CACHE] Invalidação completa. {deletedCount} entradas de inferência removidas do Redis.");
    }
}
