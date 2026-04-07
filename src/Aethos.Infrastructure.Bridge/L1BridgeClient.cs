using Nethereum.Web3;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Aethos.Infrastructure.Bridge;

/// <summary>
/// Sprint 45: L1 Solidity Bridge Client.
/// Envia o StateRoot da Layer 2 e os Proof of Reasoning (PoR) para o contrato L1 (Sepolia).
/// </summary>
public class L1BridgeClient
{
    private readonly Web3 _web3;
    private readonly ILogger<L1BridgeClient> _logger;
    private readonly string _bridgeContractAddress;

    public L1BridgeClient(string rpcUrl, string privateKey, string bridgeContractAddress, ILogger<L1BridgeClient> logger)
    {
        var account = new Nethereum.Web3.Accounts.Account(privateKey);
        _web3 = new Web3(account, rpcUrl);
        _bridgeContractAddress = bridgeContractAddress;
        _logger = logger;
    }

    /// <summary>
    /// Faz o commit atômico do bloco L2 na Layer 1. 
    /// Este é o ponto final da "Censura de Finalidade" da Aethos Ledger.
    /// </summary>
    public async Task CommitL2BlockToL1Async(long blockNumber, string stateRoot, string porHash)
    {
        _logger.LogInformation($"[BRIDGE-L1] Iniciando commit do Bloco #{blockNumber} no Ethereum...");

        try
        {
            // Nota: Em uma implementação real, usaríamos o Nethereum.Generator para criar as classes do contrato.
            // Aqui simulamos a chamada via Function Call genérica.
            var contract = _web3.Eth.GetContract("{{ABI_PLACEHOLDER}}", _bridgeContractAddress);
            var commitFunction = contract.GetFunction("commitState");

            var txHash = await commitFunction.SendTransactionAsync(
                _web3.TransactionManager.Account.Address,
                new Nethereum.Hex.HexTypes.HexBigInteger(500000), // Gas Limit
                new Nethereum.Hex.HexTypes.HexBigInteger(0),     // Value (Wei)
                blockNumber,
                stateRoot,
                porHash
            );

            _logger.LogCritical($"[BRIDGE-L1] BLOCADO! Hash da transação L1: {txHash}");
        }
        catch (System.Exception ex)
        {
            _logger.LogError($"[BRIDGE-L1] FALHA NO COMMIT L1: {ex.Message}");
            throw; // Politica de Retry (Sprint 41) entrará em ação aqui.
        }
    }
}
