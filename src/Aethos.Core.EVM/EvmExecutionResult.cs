using Aethos.Domain.ValueObjects;

namespace Aethos.Core.EVM;

public readonly record struct EvmExecutionResult
{
    public bool IsSuccess { get; }
    public decimal GasUsed { get; }
    public string StateRoot { get; }
    public ResultHash? PoRHash { get; }
    public string RevertReason { get; }

    private EvmExecutionResult(bool success, decimal gasUsed, string stateRoot, ResultHash? porHash, string revertReason)
    {
        IsSuccess = success;
        GasUsed = gasUsed;
        StateRoot = stateRoot;
        PoRHash = porHash;
        RevertReason = revertReason;
    }

    public static EvmExecutionResult Success(decimal gasUsed, string stateRoot, ResultHash? porHash = null) 
        => new EvmExecutionResult(true, gasUsed, stateRoot, porHash, string.Empty);

    public static EvmExecutionResult Revert(string reason) 
        => new EvmExecutionResult(false, 0m, string.Empty, null, reason);
}
