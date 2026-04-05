using System;
using System.Threading;
using System.Threading.Tasks;
using Aethos.Domain.Entities;
using Aethos.Domain.Exceptions;
using Aethos.Domain.Interfaces;

namespace Aethos.Core.EVM;

public class EvmTransactionProcessor
{
    private readonly IStateDb _stateDb;

    public EvmTransactionProcessor(IStateDb stateDb)
    {
        _stateDb = stateDb;
    }

    public async Task<EvmExecutionResult> ProcessTransactionAsync(TransactionEntity transaction, CancellationToken ct = default)
    {
        try
        {
            if (transaction.Value < 0.00000000001m) 
            {
                throw new InsufficientGasException($"A transação {transaction.Hash} não forneceu gás base suficiente.");
            }

            byte[] stateKey = System.Text.Encoding.UTF8.GetBytes($"tx_state_{transaction.Hash}");
            byte[] stateVal = System.Text.Encoding.UTF8.GetBytes("processed_by_aethos");

            await _stateDb.SetAsync(stateKey, stateVal, ct);
            await _stateDb.CommitAsync(ct);

            string mockStateRoot = "0x" + new string('e', 64);
            return EvmExecutionResult.Success(gasUsed: 0.002m, stateRoot: mockStateRoot);
        }
        catch (InsufficientGasException ex)
        {
            return EvmExecutionResult.Revert(ex.Message);
        }
        catch (Exception ex)
        {
            return EvmExecutionResult.Revert($"VM Falha de Segurança Crítica: {ex.Message}");
        }
    }
}
