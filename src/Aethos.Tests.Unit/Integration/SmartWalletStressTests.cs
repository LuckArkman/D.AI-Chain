using Xunit;
using FluentAssertions;
using Aethos.Domain.ValueObjects;
using Aethos.Domain.Entities;
using Aethos.Core.EVM;
using Aethos.Core.EVM.Wallets;
using Aethos.Core.Persistence.RocksDB;
using System;
using System.Threading.Tasks;

namespace Aethos.Tests.Unit.Integration;

public class SmartWalletStressTests
{
    [Fact]
    public async Task AiSmartWallet_WhenLimitExceeded_ShouldBlockTransaction()
    {
        var store = new RocksDbStore("stress_test_db");
        var processor = new EvmTransactionProcessor(store);
        var address = ContractAddress.Create("0x1234567890123456789012345678901234567890");
        var thresholds = GuardianThreshold.Create(0.5m, 2.0m);
        
        var wallet = new AiSmartWallet(address, thresholds, processor);
        var tx = new TransactionEntity("0xTxHash", address, address, 1.0m, Array.Empty<byte>());

        Func<Task> act = async () => await wallet.ExecuteAsync(tx);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
