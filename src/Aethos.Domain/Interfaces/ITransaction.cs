using Aethos.Domain.ValueObjects;

namespace Aethos.Domain.Interfaces;

public interface ITransaction
{
    string Hash { get; }
    ContractAddress From { get; }
    ContractAddress To { get; }
    decimal Value { get; }
    byte[] Data { get; }
}
