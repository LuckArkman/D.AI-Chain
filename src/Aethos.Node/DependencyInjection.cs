using Microsoft.Extensions.DependencyInjection;
using Aethos.Domain.Interfaces;
using Aethos.Core.Persistence.RocksDB;
using Aethos.Core.EVM;

namespace Aethos.Node;

public static class DependencyInjection
{
    public static IServiceCollection AddAethosNode(this IServiceCollection services, string dataDirectory)
    {
        services.AddSingleton<IStateDb>(sp => new RocksDbStore(dataDirectory));
        services.AddScoped<EvmTransactionProcessor>();
        return services;
    }
}
