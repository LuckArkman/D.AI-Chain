using MessagePack;
using System;

namespace Aethos.Core.AI.LSTM;

/// <summary>
/// Sprint 20: As trilhas para explicabilidade auditável.
/// </summary>
[MessagePackObject]
public class ActivationTrace
{
    [Key(0)]
    public int LayerIndex { get; set; }
    
    [Key(1)]
    public long Timestamp { get; set; }
    
    [Key(2)]
    public byte[] CompressedState { get; set; }

    public ActivationTrace() { }

    public ActivationTrace(int layerIndex, byte[] compressedState)
    {
        LayerIndex = layerIndex;
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        CompressedState = compressedState;
    }
}
