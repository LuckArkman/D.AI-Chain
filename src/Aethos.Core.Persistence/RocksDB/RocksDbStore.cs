using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Aethos.Domain.Interfaces;

namespace Aethos.Core.Persistence.RocksDB;

public class RocksDbStore : IStateDb, IDisposable
{
    private readonly string _dbPath;
    private readonly ConcurrentDictionary<string, byte[]> _kvStore;

    public RocksDbStore(string dbPath)
    {
        _dbPath = dbPath;
        _kvStore = new ConcurrentDictionary<string, byte[]>();
    }

    public Task<byte[]> GetAsync(byte[] key, CancellationToken ct = default)
    {
        string base64Key = Convert.ToBase64String(key);
        if (_kvStore.TryGetValue(base64Key, out var value))
        {
            return Task.FromResult(value);
        }
        return Task.FromResult(Array.Empty<byte>());
    }

    public Task SetAsync(byte[] key, byte[] value, CancellationToken ct = default)
    {
        string base64Key = Convert.ToBase64String(key);
        _kvStore[base64Key] = value;
        return Task.CompletedTask;
    }

    public Task CommitAsync(CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _kvStore.Clear();
    }
}
