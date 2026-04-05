using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aethos.Core.EVM;

namespace Aethos.Presentation.RPC.Controllers;

[ApiController]
[Route("")]
public class EthRpcController : ControllerBase
{
    private readonly EvmTransactionProcessor _processor;

    public EthRpcController(EvmTransactionProcessor processor)
    {
        _processor = processor;
    }

    [HttpPost]
    public async Task<IActionResult> HandleJsonRpc([FromBody] JsonRpcRequest request)
    {
        switch (request.Method.ToLowerInvariant())
        {
            case "eth_sendrawtransaction":
                return Ok(new { jsonrpc = "2.0", id = request.Id, result = "0x123MockHashTransactionEVM" });
            case "eth_chainid":
                return Ok(new { jsonrpc = "2.0", id = request.Id, result = "0xAE7" });
            case "aethos_getpor":
                string hashTx = request.Params != null && request.Params.Length > 0 ? request.Params[0].ToString()! : "";
                return Ok(new { jsonrpc = "2.0", id = request.Id, result = $"PoR gerado para TX: {hashTx}" });
            default:
                return BadRequest(new { jsonrpc = "2.0", id = request.Id, error = new { code = -32601, message = "Method not found" } });
        }
    }
}

public class JsonRpcRequest
{
    public string JsonRpc { get; set; } = "2.0";
    public string Method { get; set; } = string.Empty;
    public object[] Params { get; set; } = [];
    public int Id { get; set; } = 1;
}
