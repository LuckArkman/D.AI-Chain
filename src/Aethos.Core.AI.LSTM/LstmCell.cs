using Aethos.Math.FixedPoint;

namespace Aethos.Core.AI.LSTM;

public class LstmCell
{
    private readonly int _hiddenSize;
    public FixedPointMatrix WeightsForget { get; }
    public FixedPointMatrix WeightsInput { get; }
    public FixedPointMatrix WeightsCandidate { get; }
    public FixedPointMatrix WeightsOutput { get; }

    public LstmCell(int hiddenSize, FixedPointMatrix wf, FixedPointMatrix wi, FixedPointMatrix wc, FixedPointMatrix wo)
    {
        _hiddenSize = hiddenSize;
        WeightsForget = wf;
        WeightsInput = wi;
        WeightsCandidate = wc;
        WeightsOutput = wo;
    }

    public LstmState Forward(FixedPointVector input, LstmState previousState)
    {
        FixedPointVector forgetProduct = FixedPointMatrix.MatVecMul(WeightsForget, input);
        FixedPointVector inputProduct = FixedPointMatrix.MatVecMul(WeightsInput, input);
        FixedPointVector candidateProduct = FixedPointMatrix.MatVecMul(WeightsCandidate, input);
        FixedPointVector outputProduct = FixedPointMatrix.MatVecMul(WeightsOutput, input);

        FixedPointInt128[] newCellData = new FixedPointInt128[_hiddenSize];
        FixedPointInt128[] newHiddenData = new FixedPointInt128[_hiddenSize];

        for (int i = 0; i < _hiddenSize; i++)
        {
            var f_t     = ActivationLUT.Sigmoid(forgetProduct[i] + previousState.HiddenState[i]);
            var i_t     = ActivationLUT.Sigmoid(inputProduct[i] + previousState.HiddenState[i]);
            var c_tilde = ActivationLUT.Tanh(candidateProduct[i] + previousState.HiddenState[i]);
            var o_t     = ActivationLUT.Sigmoid(outputProduct[i] + previousState.HiddenState[i]);

            newCellData[i] = (f_t * previousState.CellState[i]) + (i_t * c_tilde);
            newHiddenData[i] = o_t * ActivationLUT.Tanh(newCellData[i]);
        }

        return new LstmState(new FixedPointVector(newHiddenData), new FixedPointVector(newCellData));
    }
}
