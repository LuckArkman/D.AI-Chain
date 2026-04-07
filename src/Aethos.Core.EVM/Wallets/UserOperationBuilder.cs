using System;
using Aethos.Domain.ValueObjects;
using Aethos.Domain.Entities;

namespace Aethos.Core.EVM.Wallets;

public static class UserOperationBuilder
{
    public static TransactionEntity BuildAutonomousTx(ContractAddress from, ContractAddress to, decimal valueEth, byte[] executionData)
    {
        string mockHash = "0x" + Guid.NewGuid().ToString("N").PadRight(64, '0');
        return new TransactionEntity(mockHash.Substring(0, 66), from, to, valueEth, executionData);
    }
}
