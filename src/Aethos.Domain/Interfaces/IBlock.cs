using System.Collections.Generic;
using Aethos.Domain.ValueObjects;

namespace Aethos.Domain.Interfaces;

public interface IBlock
{
    ulong Number { get; }
    string ParentHash { get; }
    string StateRoot { get; }
    IReadOnlyList<ITransaction> Transactions { get; }
}
