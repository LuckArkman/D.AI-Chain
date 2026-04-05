using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Aethos.Presentation.RPC.Hubs;

public class AdminSignalRHub : Hub
{
    public async Task SubscribeToNetworkHealth()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "NetworkAdmins");
        await Clients.Caller.SendAsync("ReceiveSystemAlert", "Subscrito aos eventos da Aethos L2");
    }

    public async Task BroadcastNewBlock(string blockHash, string stateRoot)
    {
        await Clients.Group("NetworkAdmins").SendAsync("OnBlockFinalized", new { blockHash, stateRoot });
    }
    
    public async Task BroadcastInferenceTrace(string callerAddress, string porHash)
    {
        await Clients.Group("NetworkAdmins").SendAsync("OnAiContractExecuted", new { callerAddress, porHash });
    }
}
