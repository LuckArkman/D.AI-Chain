using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Aethos.Presentation.RPC.Hubs;

/// <summary>
/// Sprint 50: Hub de Saúde da Rede.
/// Transmite métricas de consenso e divergência em tempo real para o Dashboard Admin.
/// </summary>
public class NetworkHealthHub : Hub
{
    public async Task SendHealthUpdate(string status, double divergenceRate)
    {
        await Clients.All.SendAsync("ReceiveHealthUpdate", new { 
            Status = status, 
            DivergenceRate = divergenceRate,
            Timestamp = System.DateTime.UtcNow 
        });
    }
}

/// <summary>
/// Sprint 50: Hub de Feed de Transações.
/// Streaming de transações processadas (Mempool -> Block).
/// </summary>
public class TransactionFeedHub : Hub
{
    public async Task BroadcastTransaction(string txHash, string from, string to, decimal value)
    {
        await Clients.All.SendAsync("NewTransaction", new {
            Hash = txHash,
            From = from,
            To = to,
            Value = value
        });
    }
}
