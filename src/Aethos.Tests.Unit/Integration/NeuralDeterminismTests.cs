using Xunit;
using FluentAssertions;
using Aethos.Math.FixedPoint;
using Aethos.Core.AI.LSTM;
using Aethos.Core.AI.LSTM.Engine;
using System.Threading.Tasks;

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
    public async Task NeuralEngine_SameInput_ProducesSamePoR()
    {
        int hiddenSize = 64; 
        var layers = new LstmCell[4] { 
            CreateMockCell(hiddenSize), CreateMockCell(hiddenSize), 
            CreateMockCell(hiddenSize), CreateMockCell(hiddenSize) 
        };
        var engine = new NeuralEngine(layers, hiddenSize);
        var input = new FixedPointVector(new FixedPointInt128[hiddenSize]);

        var result1 = await engine.ExecuteAethosRunAsync(5, input);
        var result2 = await engine.ExecuteAethosRunAsync(5, input);

        result1.PoR.Value.Should().Be(result2.PoR.Value);
    }
}
