using System;

namespace Aethos.Math.FixedPoint;

public enum ActivationFunction { Tanh, Sigmoid, ReLU }

public static class ActivationLUT
{
    private const int LUT_SIZE = 65536;
    private const double MIN_VAL = -8.0;
    private const double MAX_VAL = 8.0;
    private const double RANGE = MAX_VAL - MIN_VAL;
    private const double STEP = RANGE / (LUT_SIZE - 1);

    private static readonly FixedPointInt128[] _tanhLUT;
    private static readonly FixedPointInt128[] _sigmoidLUT;

    static ActivationLUT()
    {
        _tanhLUT = new FixedPointInt128[LUT_SIZE];
        _sigmoidLUT = new FixedPointInt128[LUT_SIZE];

        for (int i = 0; i < LUT_SIZE; i++)
        {
            double x = MIN_VAL + (i * STEP);
            _tanhLUT[i] = FixedPointInt128.FromDouble(System.Math.Tanh(x));
            _sigmoidLUT[i] = FixedPointInt128.FromDouble(1.0 / (1.0 + System.Math.Exp(-x)));
        }
    }

    private static int MapToIndex(FixedPointInt128 value)
    {
        double x = value.ToDouble();
        if (x <= MIN_VAL) return 0;
        if (x >= MAX_VAL) return LUT_SIZE - 1;

        int index = (int)System.Math.Round((x - MIN_VAL) / STEP);
        
        if (index < 0) return 0;
        if (index >= LUT_SIZE) return LUT_SIZE - 1;
        
        return index;
    }

    public static FixedPointInt128 Tanh(FixedPointInt128 x) => _tanhLUT[MapToIndex(x)];

    public static FixedPointInt128 Sigmoid(FixedPointInt128 x) => _sigmoidLUT[MapToIndex(x)];

    public static FixedPointInt128 ReLU(FixedPointInt128 x)
    {
        return x.ToDouble() > 0 ? x : FixedPointInt128.FromInt(0);
    }
}
