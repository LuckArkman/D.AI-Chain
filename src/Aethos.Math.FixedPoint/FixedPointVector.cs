namespace Aethos.Math.FixedPoint;

public readonly struct FixedPointVector
{
    public int Length { get; }
    private readonly FixedPointInt128[] _data;

    public FixedPointVector(FixedPointInt128[] data)
    {
        Length = data.Length;
        _data = data;
    }

    public FixedPointInt128 this[int index] => _data[index];
}
