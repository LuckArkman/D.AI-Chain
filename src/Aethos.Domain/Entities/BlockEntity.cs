using System.Collections.Generic;
using Aethos.Domain.Interfaces;

namespace Aethos.Domain.Entities;

public class BlockEntity : IBlock
{
    public ulong Number { get; }
    public string ParentHash { get; }
    public string StateRoot { get; }
    public IReadOnlyList<ITransaction> Transactions { get; }

    public BlockEntity(ulong number, string parentHash, string stateRoot, IReadOnlyList<ITransaction> transactions)
    {
        Number = number;
        ParentHash = parentHash;
        StateRoot = stateRoot;
        Transactions = transactions;
    }
}
