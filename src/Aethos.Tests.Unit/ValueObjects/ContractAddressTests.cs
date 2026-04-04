namespace Aethos.Tests.Unit.ValueObjects;

using Aethos.Domain.ValueObjects;
using FluentAssertions;

public class ContractAddressTests
{
    [Fact]
    public void Create_ValidAddress_ReturnsInstance()
    {
        var address = "0x1234567890abcdef1234567890abcdef12345678";
        var sut = ContractAddress.Create(address);
        sut.Value.Should().Be(address);
    }

    [Fact]
    public void Create_EmptyAddress_ThrowsArgumentException()
    {
        Action act = () => ContractAddress.Create("");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_InvalidLength_ThrowsArgumentException()
    {
        Action act = () => ContractAddress.Create("0x123");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_No0xPrefix_ThrowsArgumentException()
    {
        Action act = () => ContractAddress.Create("1x1234567890abcdef1234567890abcdef12345678");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_InvalidHexChars_ThrowsArgumentException()
    {
        Action act = () => ContractAddress.Create("0x1234567890abcdef1234567890abcdef1234567z");
        act.Should().Throw<ArgumentException>();
    }
}
