using System;
using System.Diagnostics.CodeAnalysis;

namespace Aethos.Math.FixedPoint;

public readonly struct FixedPointInt128 : IEquatable<FixedPointInt128>
{
    private readonly Int128 _raw;
    private const int FRACTION_BITS = 44;
    private static readonly Int128 SCALE = (Int128)1 << FRACTION_BITS;

    private FixedPointInt128(Int128 raw) => _raw = raw;

    public static FixedPointInt128 FromInt(long value) => new FixedPointInt128((Int128)value << FRACTION_BITS);

    public static FixedPointInt128 FromDouble(double value) => new FixedPointInt128((Int128)(value * (double)SCALE));

    public static FixedPointInt128 operator +(FixedPointInt128 a, FixedPointInt128 b) => new FixedPointInt128(a._raw + b._raw);

    public static FixedPointInt128 operator -(FixedPointInt128 a, FixedPointInt128 b) => new FixedPointInt128(a._raw - b._raw);

    public static FixedPointInt128 operator *(FixedPointInt128 a, FixedPointInt128 b)
    {
        Int128 product = a._raw * b._raw;
        return new FixedPointInt128(product >> FRACTION_BITS);
    }

    public FixedPointInt128 CheckedMultiply(FixedPointInt128 other)
    {
        try
        {
            checked
            {
                Int128 product = _raw * other._raw;
                return new FixedPointInt128(product >> FRACTION_BITS);
            }
        }
        catch (OverflowException)
        {
            throw new Exception("Multiplicação excedeu o limite do motor determinístico.");
        }
    }

    public double ToDouble() => (double)_raw / (double)SCALE;

    public override string ToString() => ToDouble().ToString("F14");

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is FixedPointInt128 other && _raw == other._raw;

    public bool Equals(FixedPointInt128 other) => _raw == other._raw;

    public override int GetHashCode() => _raw.GetHashCode();

    public static bool operator ==(FixedPointInt128 a, FixedPointInt128 b) => a._raw == b._raw;

    public static bool operator !=(FixedPointInt128 a, FixedPointInt128 b) => a._raw != b._raw;
}
