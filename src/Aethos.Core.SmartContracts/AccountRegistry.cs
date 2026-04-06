using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Aethos.Core.SmartContracts;

/// <summary>
/// Tipos de Contas nativas indexadas no State Root da Aethos L2.
/// </summary>
public enum AccountType
{
    ExternallyOwnedAccount = 0, // Metamask user com chaves Criptográficas EC normais
    SmartWallet = 1,            // MultiSig ou abstração básica (ERC-4337 standard)
    AiWallet = 2                // Carteira acoplada ao Motor LSTM Determinístico (Transações autônomas)
}

/// <summary>
/// Sprint 32: Account Registry.
/// Indexador global de carteiras do ledger, separando Contas Ocas de Contratos Autônomos.
/// </summary>
public class AccountRegistry
{
    private readonly ConcurrentDictionary<string, AccountType> _registry;

    public AccountRegistry()
    {
        _registry = new ConcurrentDictionary<string, AccountType>();
        
        // Bootstrapping mock da Genesis Block
        _registry.TryAdd("0xvalidator01", AccountType.SmartWallet);
        _registry.TryAdd("0xaigenesisroot", AccountType.AiWallet);
    }

    /// <summary>
    /// Cadastra a natureza da conta no RocksDB logic. 
    /// (Ocorre no Deploy do Contract Creation payload).
    /// </summary>
    public Task RegisterAccountAsync(string address, AccountType type)
    {
        _registry.AddOrUpdate(address.ToLowerInvariant(), type, (key, old) => type);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Devolve rapidamente se o endereço tem ganchos com a LSTM ou se resolve apenas no fluxo Base.
    /// </summary>
    public Task<AccountType> GetAccountTypeAsync(string address)
    {
        if (_registry.TryGetValue(address.ToLowerInvariant(), out var type))
        {
            return Task.FromResult(type);
        }

        // De fallback, se não estiver cravada que foi deployed, é sempre um EOA comum (Ethereum Yellow Paper)
        return Task.FromResult(AccountType.ExternallyOwnedAccount);
    }
}
