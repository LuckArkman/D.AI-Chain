namespace Aethos.Tests.Unit.ValueObjects;

using Aethos.Domain.ValueObjects;
using FluentAssertions;

public class GuardianThresholdTests
{
    [Fact]
    public void Create_ValidLimits_ReturnsInstance()
    {
        var sut = GuardianThreshold.Create(1.5m, 5.0m);
        sut.AutonomousLimit.Should().Be(1.5m);
        sut.MultiSigLimit.Should().Be(5.0m);
    }

    [Fact]
    public void Create_NegativeAutonomousLimit_ThrowsArgumentException()
    {
        Action act = () => GuardianThreshold.Create(-1m, 5.0m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_MultiSigLowerThanAutonomous_ThrowsArgumentException()
    {
        Action act = () => GuardianThreshold.Create(5.0m, 1.0m);
        act.Should().Throw<ArgumentException>();
    }
}
