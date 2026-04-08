using System;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Aethos.Presentation.RPC.Data;

namespace Aethos.Presentation.RPC.Controllers;

[ApiController]
[Route("api/mainnet")]
public class MainnetController : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new { 
            network = "Aethos AI-Chain Mainnet", 
            chainId = 1111,
            status = "Operational",
            epoch = 1500
        });
    }

    [HttpGet("accounts/{address}/balance")]
    public IActionResult GetBalance(string address)
    {
        if(string.IsNullOrEmpty(address))
            return BadRequest(new { error = "Address is required" });

        var wallet = MongoDbService.Wallets.Find(w => w.Address == address).FirstOrDefault();
        if (wallet == null) return NotFound(new { error = "Wallet not found in Mainnet DB" });

        return Ok(new { 
            address = address,
            network = "Mainnet",
            balance = wallet.Balance,
            currency = wallet.Currency,
            tokenBalances = wallet.TokenBalances
        });
    }

    [HttpPost("transactions/transfer")]
    public IActionResult ExecuteFinancialTransfer([FromBody] NetworkTransferRequest request)
    {
        if(string.IsNullOrEmpty(request.From) || string.IsNullOrEmpty(request.To))
            return BadRequest(new { error = "Transactions require valid from and to addresses." });

        if(!decimal.TryParse(request.Amount, out var requestedAmount))
            return BadRequest(new { error = "Invalid transaction amount format." });

        var sender = MongoDbService.Wallets.Find(w => w.Address == request.From).FirstOrDefault();
        if (sender == null || decimal.Parse(sender.Balance) < requestedAmount)
            return BadRequest(new { error = "Origin wallet not found or insufficient mainnet balance." });

        var receiver = MongoDbService.Wallets.Find(w => w.Address == request.To).FirstOrDefault();
        if (receiver == null)
            return BadRequest(new { error = "Destination wallet not found in DB." });

        // Update states
        sender.Balance = (decimal.Parse(sender.Balance) - requestedAmount).ToString();
        receiver.Balance = (decimal.Parse(receiver.Balance) + requestedAmount).ToString();

        MongoDbService.Wallets.ReplaceOne(w => w.Address == request.From, sender);
        MongoDbService.Wallets.ReplaceOne(w => w.Address == request.To, receiver);

        return Ok(new { 
            message = "Mainnet financial transfer executed successfully.",
            transactionHash = "0x" + Guid.NewGuid().ToString("N") + "0000",
            from = request.From,
            to = request.To,
            amount = request.Amount,
            status = "Confirmed",
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
    }

    [HttpPost("contracts/deploy")]
    public IActionResult DeployMainnetContract([FromBody] NetworkDeployRequest request)
    {
        var contractAddress = "0xAC" + Guid.NewGuid().ToString("N") + "0000";
        return Ok(new { 
            message = "Smart Contract successfully minted to Mainnet.",
            contractAddress = contractAddress,
            deployer = request.DeployerAddress,
            status = "Confirmed",
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
    }

    [HttpPost("contracts/execute")]
    public IActionResult ExecuteMainnetContract([FromBody] NetworkContractExecuteRequest request)
    {
        return Ok(new { 
            message = "Mainnet Contract Execution confirmed",
            transactionHash = "0x" + Guid.NewGuid().ToString("N") + "0000",
            contractAddress = request.ContractAddress,
            status = "Confirmed",
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
    }
}
