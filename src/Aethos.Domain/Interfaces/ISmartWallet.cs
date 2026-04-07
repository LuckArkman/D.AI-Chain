using System.Threading.Tasks;
using Aethos.Domain.ValueObjects;

namespace Aethos.Domain.Interfaces;

public interface ISmartWallet
{
    ContractAddress Address { get; }
    Task<bool> ValidateOwnerAsync(string signature, byte[] data);
    Task ExecuteAsync(ITransaction transaction);
}
