using System.Threading;
using System.Threading.Tasks;
using Aethos.Domain.ValueObjects;

namespace Aethos.Domain.Interfaces;

public interface IAiContract
{
    ContractAddress Address { get; }
    Task<ResultHash> ExecuteInferenceAsync(byte[] inputData, CancellationToken ct = default);
}
