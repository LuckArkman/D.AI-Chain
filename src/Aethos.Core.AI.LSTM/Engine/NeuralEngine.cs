using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aethos.Domain.ValueObjects;
using Aethos.Math.FixedPoint;
using Aethos.Core.AI.LSTM.Explicability;

namespace Aethos.Core.AI.LSTM.Engine;

public class NeuralEngine
{
    private readonly LstmCell[] _layers;
    private readonly int _hiddenSize;

    public NeuralEngine(LstmCell[] layers, int hiddenSize = 256)
    {
        if (layers.Length != 4)
            throw new ArgumentException("A Arquitetura Aethos Layer 2 exige 4 camadas LSTM.");
            
        _layers = layers;
        _hiddenSize = hiddenSize;
    }

    public async Task<(FixedPointVector FinalOutput, ResultHash PoR)> ExecuteAethosRunAsync(int timeSteps, FixedPointVector staticInput, CancellationToken ct = default)
    {
        var states = new LstmState[4];
        for (int i = 0; i < 4; i++)
        {
            var zeroVec = new FixedPointVector(new FixedPointInt128[_hiddenSize]); 
            states[i] = new LstmState(zeroVec, zeroVec);
        }

        var executionTrace = new List<LstmState[]>();

        for (int t = 0; t < timeSteps; t++)
        {
            ct.ThrowIfCancellationRequested();
            var currentFrameStates = new LstmState[4];
            FixedPointVector currentInput = staticInput;

            for (int layer = 0; layer < 4; layer++)
            {
                var newState = _layers[layer].Forward(currentInput, states[layer]);
                states[layer] = newState;
                currentFrameStates[layer] = newState;
                currentInput = newState.HiddenState; 
            }

            executionTrace.Add(currentFrameStates);
            await Task.Yield();
        }

        ResultHash por = ProofOfReasoningGenerator.GeneratePoR(executionTrace);
        return (states[3].HiddenState, por);
    }
}
