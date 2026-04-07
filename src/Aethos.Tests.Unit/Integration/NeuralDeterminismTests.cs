using Xunit;
using FluentAssertions;
using Aethos.Math.FixedPoint;
using Aethos.Core.AI.LSTM;

namespace Aethos.Tests.Unit.Integration;

public class NeuralDeterminismTests
{
    private LstmCell CreateMockCell(int size)
    {
        var data = new FixedPointInt128[size * size];
        for (int i = 0; i < data.Length; i++) data[i] = FixedPointInt128.FromDouble(0.1);
        var mat = new FixedPointMatrix(size, size, data);
        return new LstmCell(size, mat, mat, mat, mat);
    }

    [Fact]
    public void LstmNetwork_SameInput_ProducesSameExactOutput()
    {
        int hiddenSize = 64; 
        var layers = new LstmCell[4] { 
            CreateMockCell(hiddenSize), CreateMockCell(hiddenSize), 
            CreateMockCell(hiddenSize), CreateMockCell(hiddenSize) 
        };
        var network = new LstmNetwork(layers, hiddenSize);
        var input = new FixedPointVector(new FixedPointInt128[hiddenSize]);

        var states1 = new LstmState[4];
        var states2 = new LstmState[4];

        for (int i = 0; i < 4; i++) {
            var zeroVec = new FixedPointVector(new FixedPointInt128[hiddenSize]);
            states1[i] = new LstmState(zeroVec, zeroVec);
            states2[i] = new LstmState(zeroVec, zeroVec);
        }

        var result1 = network.ForwardWithTrace(input, ref states1).Output;
        var result2 = network.ForwardWithTrace(input, ref states2).Output;

        // Validação estrita bit-a-bit contra CPU Floating Point drift
        result1[0].Should().Be(result2[0]);
    }
}
