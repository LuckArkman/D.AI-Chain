using HashLib4CSharp.Interfaces;
using HashLib4CSharp.Base;
using Aethos.Math.FixedPoint;
using Aethos.Domain.ValueObjects;
using System;
using System.Text;

namespace Aethos.Core.AI.LSTM;

/// <summary>
/// Sprint 21: Proof of Reasoning - Compactação e Hash da prova auditável.
/// </summary>
public class ProofOfReasoning
{
    public ResultHash Hash { get; }

    private ProofOfReasoning(ResultHash hash)
    {
        Hash = hash;
    }

    /// <summary>
    /// Aethos gera o ResultHash da rede em Keccak256 sobre a resposta probabilística da softmax final.
    /// </summary>
    public static ProofOfReasoning Generate(FixedPointVector finalSoftmax, ActivationTrace[] traces)
    {
        var keccak = HashLib4CSharp.Base.HashFactory.Crypto.CreateKeccak_256();
        
        // Add softmax determinístico para o hash materializado (string exata F8 mitigando flutuação)
        for (int i = 0; i < finalSoftmax.Length; i++) 
        {
            var val = finalSoftmax[i];
            var b = Encoding.UTF8.GetBytes(val.ToDouble().ToString("F8"));
            keccak.TransformBytes(b);
        }

        // Incorpora a trilha criptográfica camada a camada (Pesos influentes + Timestamp)
        foreach(var trace in traces) 
        {
            if (trace.CompressedState != null)
                keccak.TransformBytes(trace.CompressedState);
            
            keccak.TransformBytes(BitConverter.GetBytes(trace.LayerIndex));
            keccak.TransformBytes(BitConverter.GetBytes(trace.Timestamp));
        }

        var result = keccak.TransformFinal();
        string hex = "0x" + result.ToString().ToLowerInvariant();

        return new ProofOfReasoning(ResultHash.Create(hex)); 
    }
}
