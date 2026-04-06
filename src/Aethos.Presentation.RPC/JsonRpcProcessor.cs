using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System;

namespace Aethos.Presentation.RPC;

/// <summary>
/// Sprint 23/24/25: Pipeline ASP.NET de JsonRpc processual HTTP Nativo.
/// Escuta chamadas HTTP Puras (MetaMask RPC compatível).
/// </summary>
public class JsonRpcProcessor
{
    private readonly RequestDelegate _next;
    private readonly EthMethodRouter _ethRouter = new();
    private readonly AethosMethodRouter _aethosRouter = new();

    public JsonRpcProcessor(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/" && context.Request.Method == HttpMethods.Post)
        {
            context.Response.ContentType = "application/json";
            
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            
            try 
            {
                var req = JsonSerializer.Deserialize<JsonElement>(body);
                string method = req.GetProperty("method").GetString();
                int id = req.GetProperty("id").GetInt32();
                
                object result = null;
                
                switch (method)
                {
                    case "web3_clientVersion":
                        result = "Aethos/v1.0.0/deterministic-l2/dotnet8.0";
                        break;
                    case "eth_chainId":
                        result = _ethRouter.ChainId();
                        break;
                    case "eth_blockNumber":
                        result = _ethRouter.BlockNumber();
                        break;
                    case "eth_getBalance":
                        result = _ethRouter.GetBalance("", "");
                        break;
                    case "eth_estimateGas":
                        result = _ethRouter.EstimateGas(null);
                        break;
                    case "eth_getTransactionCount":
                        result = _ethRouter.GetTransactionCount("", "");
                        break;
                    case "aethos_getPoR":
                        result = _aethosRouter.GetProofOfReasoning("");
                        break;
                    case "aethos_getActivationTrace":
                        result = _aethosRouter.GetActivationTrace("");
                        break;
                    default:
                        result = "0x0";
                        break;
                }
                
                var response = new 
                {
                    jsonrpc = "2.0",
                    id = id,
                    result = result
                };
                
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch
            {
                // Format errors out of scope for mockup
            }
        }
        else
        {
            await _next(context);
        }
    }
}
