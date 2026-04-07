using Microsoft.AspNetCore.Builder;

namespace Aethos.Presentation.RPC;

/// <summary>
/// Sprint 23: Extensões do Servidor Base para configuração no Program.cs
/// </summary>
public static class JsonRpcServerExtensions
{
    /// <summary>
    /// Configura o Módulo RPC Public Facing do Aethos Ledger.
    /// </summary>
    public static IApplicationBuilder UseAethosJsonRpc(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JsonRpcProcessor>();
    }

    /// <summary>
    /// Configura o Módulo de WebSockets (Ethereum pub/sub simulado).
    /// </summary>
    public static IApplicationBuilder UseAethosWebSockets(this IApplicationBuilder builder)
    {
        builder.UseWebSockets();
        return builder.UseMiddleware<WebSocketSubscriptionManager>();
    }
}
