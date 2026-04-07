using System.Threading;
using System.Threading.Tasks;

namespace Aethos.Domain.Interfaces;

public interface IStateDb
{
    Task<byte[]> GetAsync(byte[] key, CancellationToken ct = default);
    Task SetAsync(byte[] key, byte[] value, CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
}
