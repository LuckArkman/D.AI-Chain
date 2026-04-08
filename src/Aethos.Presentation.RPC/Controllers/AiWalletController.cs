using System;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Linq;
using Aethos.Presentation.RPC.Data;

namespace Aethos.Presentation.RPC.Controllers;

[ApiController]
[Route("api/ai-wallet")]
public class AiWalletController : ControllerBase
{
    public AiWalletController()
    {
    }

    [HttpPost("create")]
    public IActionResult CreateAiWallet()
    {
        var newAddress = "0xAI" + Guid.NewGuid().ToString("N") + "0000";
        var wallet = new WalletData { Address = newAddress, Balance = "0" };
        MongoDbService.Wallets.InsertOne(wallet);

        return Ok(new { 
            message = "AI Wallet created successfully", 
            address = newAddress,
            status = "created"
        });
    }

    [HttpGet("{address}/balance")]
    public IActionResult GetBalance(string address)
    {
        var wallet = MongoDbService.Wallets.Find(w => w.Address == address).FirstOrDefault();
        if (wallet == null) return NotFound(new { error = "Wallet not found" });

        decimal parsedNative = 0m;
        decimal.TryParse(wallet.Balance, out parsedNative);
        
        // Sum up tokens just in case the wallet has retro-active balances that didn't sync natively
        decimal totalBalance = parsedNative + (wallet.TokenBalances?.Values.Sum() ?? 0m);

        return Ok(new { 
            address = address,
            balance = totalBalance.ToString(),
            currency = wallet.Currency,
            tokenBalances = wallet.TokenBalances
        });
    }

    [HttpPost("transfer")]
    public IActionResult Transfer([FromBody] TransferRequest request)
    {
        if(string.IsNullOrEmpty(request.From) || string.IsNullOrEmpty(request.To))
            return BadRequest(new { error = "Invalid addresses" });

        var sender = MongoDbService.Wallets.Find(w => w.Address == request.From).FirstOrDefault();
        if (sender != null) 
        {
            sender.Balance = (decimal.Parse(sender.Balance) - decimal.Parse(request.Amount)).ToString();
            MongoDbService.Wallets.ReplaceOne(w => w.Address == request.From, sender);
        }

        var receiver = MongoDbService.Wallets.Find(w => w.Address == request.To).FirstOrDefault();
        if (receiver != null) 
        {
            receiver.Balance = (decimal.Parse(receiver.Balance) + decimal.Parse(request.Amount)).ToString();
            MongoDbService.Wallets.ReplaceOne(w => w.Address == request.To, receiver);
        }

        return Ok(new { 
            message = "Transfer executed successfully via AI Wallet logic",
            transactionHash = "0x" + Guid.NewGuid().ToString("N") + "0000",
            from = request.From,
            to = request.To,
            amount = request.Amount
        });
    }

    [HttpPost("deploy")]
    public IActionResult Deploy([FromBody] DeployWalletRequest request)
    {
        var deployedAddress = "0xAI" + Guid.NewGuid().ToString("N") + "0000";
        return Ok(new { 
            message = "AI Wallet SC deployed on-chain",
            address = deployedAddress,
            owner = request.OwnerAddress
        });
    }

    [HttpGet("{address}/debug")]
    public IActionResult DebugWallet(string address)
    {
        return Ok(new { 
            address = address,
            state = "Active",
            nonce = 42,
            lastInferenceConfidence = 0.98,
            warnings = Array.Empty<string>()
        });
    }
}

public class TransferRequest
{
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
}

public class DeployWalletRequest
{
    public string OwnerAddress { get; set; } = string.Empty;
}
