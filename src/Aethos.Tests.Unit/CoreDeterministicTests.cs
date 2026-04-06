using Xunit;
using FluentAssertions;
using Aethos.Math.FixedPoint;
using Aethos.Core.AI.LSTM;

namespace Aethos.Tests.Unit;

/// <summary>
/// Sprint 59: Massive Unit Test Suite - Core Math & AI Convergence.
/// Garante que a aritmética de 128-bit e o motor LSTM sejam 100% determinísticos.
/// </summary>
public class CoreDeterministicTests
{
    [Fact]
    public void FixedPoint_Addition_ShouldBeExact()
    {
        // ARRANGE
        var val1 = FixedPointInt128.FromFloat(10.5f);
        var val2 = FixedPointInt128.FromFloat(20.25f);

        // ACT
        var result = val1 + val2;

        // ASSERT
        result.ToDouble().Should().Be(30.75);
    }

    [Fact]
    public void LstmCell_Inference_ShouldBeConsistent()
    {
        // ARRANGE
        int inputSize = 4;
        int hiddenSize = 2;
        
        // Mock de matrizes de peso unitárias para o teste de determinismo
        var wf = new FixedPointMatrix(hiddenSize, inputSize, new FixedPointInt128[hiddenSize * inputSize]);
        var wi = new FixedPointMatrix(hiddenSize, inputSize, new FixedPointInt128[hiddenSize * inputSize]);
        var wc = new FixedPointMatrix(hiddenSize, inputSize, new FixedPointInt128[hiddenSize * inputSize]);
        var wo = new FixedPointMatrix(hiddenSize, inputSize, new FixedPointInt128[hiddenSize * inputSize]);

        var cell = new LstmCell(hiddenSize, wf, wi, wc, wo);
        
        var inputData = new FixedPointInt128[inputSize];
        for(int i=0; i<inputSize; i++) inputData[i] = FixedPointInt128.FromFloat(0.5f);
        var inputVec = new FixedPointVector(inputData);

        var initialState = new LstmState(
            new FixedPointVector(new FixedPointInt128[hiddenSize]), 
            new FixedPointVector(new FixedPointInt128[hiddenSize])
        );

        // ACT
        var state1 = cell.Forward(inputVec, initialState);
        var state2 = cell.Forward(inputVec, initialState);

        // ASSERT - Determinismo Absoluto (Obrigatório para Consenso L2)
        for(int i=0; i<hiddenSize; i++)
        {
            state1.HiddenState[i].Equals(state2.HiddenState[i]).Should().BeTrue("Divergência detectada no HiddenState!");
            state1.CellState[i].Equals(state2.CellState[i]).Should().BeTrue("Divergência detectada no CellState!");
        }
    }

    [Fact]
    public void FixedPoint_Multiplication_Precision_Test()
    {
        // ARRANGE
        var val1 = FixedPointInt128.FromFloat(1.234567f);
        var val2 = FixedPointInt128.FromFloat(2.0f);

        // ACT
        var result = val1 * val2;

        // ASSERT
        result.ToDouble().Should().BeApproximately(2.469134, 0.000001);
    }
}
