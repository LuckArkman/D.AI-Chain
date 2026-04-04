namespace Aethos.Tests.Unit.ValueObjects;

using Aethos.Domain.ValueObjects;
using FluentAssertions;

public class ResultHashTests
{
    [Fact]
    public void Create_ValidHash_ReturnsInstance()
    {
        var hash = "0x123456789012345678901234567890123456789012345678901234567890abcd";
        var sut = ResultHash.Create(hash);
        sut.Value.Should().Be(hash);
    }
}
