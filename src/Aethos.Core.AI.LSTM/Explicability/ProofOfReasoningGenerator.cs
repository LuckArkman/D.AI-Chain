using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Aethos.Domain.ValueObjects;

namespace Aethos.Core.AI.LSTM.Explicability;

public static class ProofOfReasoningGenerator
{
    public static ResultHash GeneratePoR(List<LstmState[]> executionTrace)
    {
        using var sha256 = SHA256.Create();
        var sb = new StringBuilder();

        foreach (var frame in executionTrace)
        {
            foreach (var state in frame)
            {
                sb.Append(state.HiddenState.Length > 0 ? state.HiddenState[0].ToString() : "0");
                sb.Append('|');
            }
        }

        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
        return ResultHash.Create("0x" + System.Convert.ToHexString(hashBytes).ToLowerInvariant());
    }
}
