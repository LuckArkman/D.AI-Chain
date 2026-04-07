namespace Aethos.Tests.Unit.ValueObjects;

using Aethos.Domain.ValueObjects;
using FluentAssertions;

public class BlockHashTests
{
    [Fact]
    public void Create_ValidHash_ReturnsInstance()
    {
        var hash = "0x123456789012345678901234567890123456789012345678901234567890abcd";
        var sut = BlockHash.Create(hash);
        sut.Value.Should().Be(hash);
    }

    [Fact]
    public void Create_InvalidLength_ThrowsArgumentException()
    {
        Action act = () => BlockHash.Create("0x123");
        act.Should().Throw<ArgumentException>();
    }
}
