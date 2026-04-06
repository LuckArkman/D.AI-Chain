using Xunit;
using FluentAssertions;
using Moq;
using Aethos.Core.Consensus;
using Microsoft.Extensions.Logging;
using Aethos.Domain.ValueObjects;

namespace Aethos.Tests.Unit;

/// <summary>
/// Sprint 60: Massive Unit Test Suite 2 - Consensus & Divergence Logic.
/// Simula cenários de Slashing e divergência neural para validar a segurança da rede.
/// </summary>
public class ConsensusSecurityTests
{
    private readonly Mock<ILogger<DivergenceGuard>> _loggerMock = new();
    private const string VALID_HASH = "0x" + "a1b2c3d4e5f607182930a1b2c3d4e5f607182930a1b2c3d4e5f607182930a1b2";
    private const string OTHER_HASH = "0x" + "f9e8d7c6b5a403210987f9e8d7c6b5a403210987f9e8d7c6b5a403210987f9e8";

    [Fact]
    public void DivergenceGuard_MatchingHashes_ShouldReturnNoDivergence()
    {
        // ARRANGE
        var guard = new DivergenceGuard(_loggerMock.Object);
        var localHash = ResultHash.Create(VALID_HASH);
        string partnerHash = VALID_HASH;

        // ACT
        var result = guard.EvaluateSubmission(localHash, partnerHash, "0xvalidator");

        // ASSERT
        result.IsDivergent.Should().BeFalse("Hashes idênticos devem ser aceitos pelo consenso.");
    }

    [Fact]
    public void DivergenceGuard_MismatchedHashes_ShouldFlagDivergenceAndGenerateEvidence()
    {
        // ARRANGE
        var guard = new DivergenceGuard(_loggerMock.Object);
        var localHash = ResultHash.Create(VALID_HASH);
        string partnerHash = OTHER_HASH;

        // ACT
        var result = guard.EvaluateSubmission(localHash, partnerHash, "0xattacker");

        // ASSERT
        result.IsDivergent.Should().BeTrue("Hashes diferentes devem acionar o alerta de divergência.");
        result.MaliciousValidatorAddress.Should().Be("0xattacker");
        result.EvidencePayload.Should().StartWith("0xe", "Evidência de Slashing deve ser gerada para punição na L1.");
    }
}
