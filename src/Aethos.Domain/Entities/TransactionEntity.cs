using Aethos.Domain.Interfaces;
using Aethos.Domain.ValueObjects;

namespace Aethos.Domain.Entities;

public class TransactionEntity : ITransaction
{
    public string Hash { get; }
    public ContractAddress From { get; }
    public ContractAddress To { get; }
    public decimal Value { get; }
    public byte[] Data { get; }

    public TransactionEntity(string hash, ContractAddress from, ContractAddress to, decimal value, byte[] data)
    {
        Hash = hash;
        From = from;
        To = to;
        Value = value;
        Data = data;
    }
}
