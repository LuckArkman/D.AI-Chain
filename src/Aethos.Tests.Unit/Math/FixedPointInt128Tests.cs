using Aethos.Math.FixedPoint;
using FluentAssertions;
using Xunit;

namespace Aethos.Tests.Unit.Math;

public class FixedPointInt128Tests
{
    [Fact]
    public void Add_SimpleValues_ReturnsExactResult()
    {
        var a = FixedPointInt128.FromInt(1);
        var b = FixedPointInt128.FromInt(1);
        var result = a + b;
        result.ToDouble().Should().Be(2.0);
    }

    [Fact]
    public void Add_FractionalValues_ReturnsExactResult()
    {
        var a = FixedPointInt128.FromDouble(0.5);
        var b = FixedPointInt128.FromDouble(0.25);
        var result = a + b;
        result.ToDouble().Should().Be(0.75);
    }

    [Fact]
    public void Equality_SameBits_ReturnsTrue()
    {
        var a = FixedPointInt128.FromDouble(1.23456);
        var b = FixedPointInt128.FromDouble(1.23456);
        (a == b).Should().BeTrue();
    }
}
