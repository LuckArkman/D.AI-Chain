using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System;
using System.Text;

namespace Aethos.Presentation.RPC;

/// <summary>
/// Sprint 26: Streaming real-time de blocos Ethereum-like.
/// Notifica novas Heads à MetaMask via protocolo WSS.
/// </summary>
public class WebSocketSubscriptionManager
{
    private readonly RequestDelegate _next;
    private static readonly ConcurrentDictionary<string, WebSocket> _activeSubscriptions = new();

    public WebSocketSubscriptionManager(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Se a chamada cair na rota /ws, nós engajamos WebSockets
        if (context.Request.Path == "/ws")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var connectionId = Guid.NewGuid().ToString();
                
                _activeSubscriptions.TryAdd(connectionId, webSocket);
                System.Console.WriteLine($"[WebSockets] Nova wallet DApp conectada no nó Aethos: {connectionId}");

                await ReceiveLoop(webSocket, connectionId);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        else
        {
            await _next(context);
        }
    }

    private async Task ReceiveLoop(WebSocket webSocket, string connectionId)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            string requestMessage = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            
            // Intercepta e responde a inscrições de Dapps padrão Ethereum
            if (requestMessage.Contains("eth_subscribe"))
            {
                // Fake Subscription ID para homologação
                var responseMessage = "{\"jsonrpc\":\"2.0\",\"id\":1,\"result\":\"0xcd0c3e8ca745364ceee0fa6a1210faf5\"}";
                var bytes = Encoding.UTF8.GetBytes(responseMessage);
                await webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), 
                    WebSocketMessageType.Text, true, CancellationToken.None);
            }

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        _activeSubscriptions.TryRemove(connectionId, out _);
        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
            
        System.Console.WriteLine($"[WebSockets] Wallet desconectada: {connectionId}");
    }

    /// <summary>
    /// Evento publicador chamado pela ConsensusLayer quando o Sequencer Aethos sela e finaliza um novo bloco.
    /// Realiza PUSH broadcast notificando os dApps.
    /// </summary>
    public static async Task BroadcastNewBlockAsync(string blockHash, string blockNumberHex)
    {
        var notification = $"{{\"jsonrpc\":\"2.0\",\"method\":\"eth_subscription\",\"params\":{{\"subscription\":\"0xcd0c3e8ca745364ceee0fa6a1210faf5\",\"result\":{{\"hash\":\"{blockHash}\",\"number\":\"{blockNumberHex}\"}}}}}}";
        var bytes = Encoding.UTF8.GetBytes(notification);

        foreach (var pair in _activeSubscriptions)
        {
            if (pair.Value.State == WebSocketState.Open)
            {
                await pair.Value.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length),
                    WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
