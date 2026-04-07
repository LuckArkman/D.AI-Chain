using System;

namespace Aethos.Math.FixedPoint;

public readonly struct FixedPointMatrix
{
    public int Rows { get; }
    public int Cols { get; }
    private readonly FixedPointInt128[] _data;

    public FixedPointMatrix(int rows, int cols, FixedPointInt128[] data)
    {
        if (data.Length != rows * cols)
            throw new ArgumentException("Data length does not match rows * cols");
            
        Rows = rows;
        Cols = cols;
        _data = data;
    }

    public static FixedPointVector MatVecMul(FixedPointMatrix matrix, FixedPointVector vector)
    {
        if (matrix.Cols != vector.Length)
            throw new ArgumentException("Matrix columns must match vector length.");

        FixedPointInt128[] result = new FixedPointInt128[matrix.Rows];
        for (int i = 0; i < matrix.Rows; i++)
        {
            FixedPointInt128 sum = FixedPointInt128.FromInt(0);
            for (int j = 0; j < matrix.Cols; j++)
            {
                sum += matrix._data[i * matrix.Cols + j] * vector[j];
            }
            result[i] = sum;
        }
        return new FixedPointVector(result);
    }
}
