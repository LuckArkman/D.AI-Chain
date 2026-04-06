using StreamJsonRpc;

namespace Aethos.Presentation.RPC;

/// <summary>
/// Sprint 24: Endpoints para compatibilidade total com wallets padrão EVM (MetaMask).
/// </summary>
public class EthMethodRouter
{
    [JsonRpcMethod("eth_chainId")]
    public string ChainId()
    {
        // Hash hex do ChainID Aethos (Exemplo: 9999 -> 0x270f)
        return "0x270f"; 
    }

    [JsonRpcMethod("eth_blockNumber")]
    public string BlockNumber()
    {
        // Mocado na POC
        return "0x0"; 
    }

    [JsonRpcMethod("eth_getBalance")]
    public string GetBalance(string address, string block)
    {
        // Retorna simulado de 1 ETH/AETH para testes
        return "0x0de0b6b3a7640000"; // 1 * 10^18 em Hex 
    }

    [JsonRpcMethod("eth_estimateGas")]
    public string EstimateGas(object transactionParams)
    {
        return "0x5208"; // Standard Tx Gas (21000)
    }

    [JsonRpcMethod("eth_getTransactionCount")]
    public string GetTransactionCount(string address, string block)
    {
        return "0x0"; 
    }
}
