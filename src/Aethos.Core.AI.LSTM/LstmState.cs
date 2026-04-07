using Aethos.Math.FixedPoint;

namespace Aethos.Core.AI.LSTM;

public readonly struct LstmState
{
    public FixedPointVector HiddenState { get; }
    public FixedPointVector CellState { get; }

    public LstmState(FixedPointVector hiddenState, FixedPointVector cellState)
    {
        HiddenState = hiddenState;
        CellState = cellState;
    }
}
