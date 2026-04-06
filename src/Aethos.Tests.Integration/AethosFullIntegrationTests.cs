using Xunit;
using Testcontainers.Redis;
using System.Threading.Tasks;
using FluentAssertions;
using Aethos.Infrastructure.Cache;
using Microsoft.Extensions.Logging;
using Moq;

namespace Aethos.Tests.Integration;

/// <summary>
/// Sprint 61: Full Integration Tests.
/// Valida o pipeline completo de Cache e persistência usando Testcontainers (Docker).
/// </summary>
public class AethosFullIntegrationTests : IAsyncLifetime
{
    private readonly RedisContainer _redisContainer = new RedisBuilder().Build();
    private readonly Mock<ILogger<InferenceCache>> _loggerMock = new();

    public async Task InitializeAsync()
    {
        // Sobe um container Docker real de Redis para o teste
        await _redisContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _redisContainer.DisposeAsync();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task InferenceCache_Integration_WithRealRedis_ShouldWork()
    {
        // ARRANGE
        var connectionString = _redisContainer.GetConnectionString();
        var redisMultiplexer = StackExchange.Redis.ConnectionMultiplexer.Connect(connectionString);
        var cache = new InferenceCache(redisMultiplexer, _loggerMock.Object);
        
        string payloadHash = "test_hash_integration";
        string decision = "APPROVE_BY_AI";

        // ACT
        await cache.SetDecisionCacheAsync(payloadHash, decision);
        var cached = await cache.GetCachedDecisionAsync(payloadHash);

        // ASSERT
        cached.Should().NotBeNull();
        cached.Should().Contain(decision, "O resultado recuperado do Redis Dockerizado deve ser idêntico ao salvo.");
    }
}
