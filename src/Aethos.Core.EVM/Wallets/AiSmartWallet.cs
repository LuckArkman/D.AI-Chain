using System;
using System.Threading.Tasks;
using Aethos.Domain.Entities;
using Aethos.Domain.Interfaces;
using Aethos.Domain.ValueObjects;

namespace Aethos.Core.EVM.Wallets;

public class AiSmartWallet : ISmartWallet
{
    public ContractAddress Address { get; }
    public GuardianThreshold Thresholds { get; }
    private readonly EvmTransactionProcessor _processor;

    public AiSmartWallet(ContractAddress address, GuardianThreshold thresholds, EvmTransactionProcessor processor)
    {
        Address = address;
        Thresholds = thresholds;
        _processor = processor;
    }

    public Task<bool> ValidateOwnerAsync(string signature, byte[] data)
    {
        return Task.FromResult(true);
    }

    public async Task ExecuteAsync(ITransaction transaction)
    {
        if (transaction.Value > Thresholds.AutonomousLimit)
        {
            throw new InvalidOperationException(
                $"Transação interceptada (Guardian). O valor pretendido de {transaction.Value} ETH excede o limite estrito da Inteligência Artificial estipulado em {Thresholds.AutonomousLimit} ETH.");
        }

        var result = await _processor.ProcessTransactionAsync((TransactionEntity)transaction);
        if (!result.IsSuccess)
        {
            throw new Exception($"VM bloqueou a execução da AI Wallet: {result.RevertReason}");
        }
    }
}
