using System;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Aethos.Presentation.RPC.Data;

namespace Aethos.Presentation.RPC.Controllers;

[ApiController]
[Route("api/asset")]
public class AssetController : ControllerBase
{
    public AssetController()
    {
    }

    [HttpPost("create")]
    public IActionResult CreateCryptoAsset([FromBody] AssetCreateRequest request)
    {
        if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Symbol))
            return BadRequest(new { error = "O nome e a sigla (symbol) são requeridos." });

        if (request.Supply <= 0)
            return BadRequest(new { error = "O supply máximo deve ser maior que zero." });

        if (Math.Round(request.Supply, 4) != request.Supply)
            return BadRequest(new { error = "O fracionamento permite no máximo 4 casas decimais após a vírgula." });

        var contractAddress = "0xAS" + Guid.NewGuid().ToString("N") + "0000";

        var assetInfo = new AssetData
        {
            ContractAddress = contractAddress,
            Name = request.Name,
            Symbol = request.Symbol,
            MaxSupply = request.Supply,
            CurrentMinted = 0
        };

        MongoDbService.Assets.InsertOne(assetInfo);

        return Ok(new { 
            message = "Criptoativo criado com sucesso na rede db", 
            contractAddress = contractAddress,
            name = assetInfo.Name,
            symbol = assetInfo.Symbol,
            maxSupply = assetInfo.MaxSupply
        });
    }

    [HttpPost("mint")]
    public IActionResult MintAsset([FromBody] AssetMintRequest request)
    {
        var assetInfo = MongoDbService.Assets.Find(a => a.ContractAddress == request.ContractAddress).FirstOrDefault();
        if (assetInfo == null)
            return NotFound(new { error = "Criptoativo não encontrado na rede db." });

        if (request.Amount <= 0)
            return BadRequest(new { error = "A quantidade a ser emitida deve ser maior que zero." });

        if (Math.Round(request.Amount, 4) != request.Amount)
            return BadRequest(new { error = "O fracionamento do token é limitado a até 4 casas decimais após a vírgula." });

        if (assetInfo.CurrentMinted + request.Amount > assetInfo.MaxSupply)
        {
            return BadRequest(new { 
                error = "Limite de supply excedido. Não é permitido emitir novos tokens deste criptoativo.",
                maxSupply = assetInfo.MaxSupply,
                currentMinted = assetInfo.CurrentMinted,
                requestedAmount = request.Amount,
                availableToMint = assetInfo.MaxSupply - assetInfo.CurrentMinted
            });
        }

        assetInfo.CurrentMinted += request.Amount;
        MongoDbService.Assets.ReplaceOne(a => a.InternalId == assetInfo.InternalId, assetInfo);

        return Ok(new { 
            message = "Emissão de tokens efetuada com sucesso via MongoDB", 
            contractAddress = request.ContractAddress,
            mintedAmount = request.Amount,
            totalMinted = assetInfo.CurrentMinted,
            remainingSupply = assetInfo.MaxSupply - assetInfo.CurrentMinted
        });
    }

    [HttpGet("{contractAddress}")]
    public IActionResult GetAssetInfo(string contractAddress)
    {
        var assetInfo = MongoDbService.Assets.Find(a => a.ContractAddress == contractAddress).FirstOrDefault();
        if (assetInfo == null)
            return NotFound(new { error = "Criptoativo não encontrado na rede db." });

        return Ok(assetInfo);
    }
    [HttpPost("mint-to-wallet")]
    public IActionResult MintToWallet([FromBody] AssetMintToWalletRequest request)
    {
        var assetInfo = MongoDbService.Assets.Find(a => a.ContractAddress == request.ContractAddress).FirstOrDefault();
        if (assetInfo == null)
            return NotFound(new { error = "Criptoativo não encontrado na rede db." });

        if (request.Amount <= 0)
            return BadRequest(new { error = "A quantidade a ser emitida deve ser maior que zero." });

        if (Math.Round(request.Amount, 4) != request.Amount)
            return BadRequest(new { error = "O fracionamento do token é limitado a até 4 casas decimais após a vírgula." });

        if (assetInfo.CurrentMinted + request.Amount > assetInfo.MaxSupply)
        {
            return BadRequest(new { 
                error = "Limite de supply excedido. Não é permitido creditar novos tokens deste criptoativo.",
                maxSupply = assetInfo.MaxSupply,
                currentMinted = assetInfo.CurrentMinted,
                requestedAmount = request.Amount
            });
        }

        var wallet = MongoDbService.Wallets.Find(w => w.Address == request.TargetWallet).FirstOrDefault();
        if (wallet == null)
            return NotFound(new { error = "Carteira de destino não encontrada." });

        // Update Token balance on wallet
        if (wallet.TokenBalances == null) wallet.TokenBalances = new System.Collections.Generic.Dictionary<string, decimal>();
        if (!wallet.TokenBalances.ContainsKey(request.ContractAddress))
            wallet.TokenBalances[request.ContractAddress] = 0;
            
        wallet.TokenBalances[request.ContractAddress] += request.Amount;

        // Update Supply
        assetInfo.CurrentMinted += request.Amount;
        
        MongoDbService.Assets.ReplaceOne(a => a.InternalId == assetInfo.InternalId, assetInfo);
        MongoDbService.Wallets.ReplaceOne(w => w.InternalId == wallet.InternalId, wallet);

        return Ok(new { 
            message = "Credito de tokens efetuado com sucesso via contrato inteligente.", 
            contractAddress = request.ContractAddress,
            targetWallet = request.TargetWallet,
            creditedAmount = request.Amount,
            walletNewBalance = wallet.TokenBalances[request.ContractAddress]
        });
    }
    [HttpPost("transfer")]
    public IActionResult TransferAsset([FromBody] AssetTransferRequest request)
    {
        if (string.IsNullOrEmpty(request.ContractAddress) || string.IsNullOrEmpty(request.FromWallet) || string.IsNullOrEmpty(request.ToWallet))
            return BadRequest(new { error = "Parâmetros inválidos." });

        if (request.Amount <= 0)
            return BadRequest(new { error = "O valor deve ser maior que zero." });

        if (Math.Round(request.Amount, 4) != request.Amount)
            return BadRequest(new { error = "O fracionamento do token é limitado a até 4 casas decimais após a vírgula." });

        var sender = MongoDbService.Wallets.Find(w => w.Address == request.FromWallet).FirstOrDefault();
        if (sender == null || sender.TokenBalances == null || !sender.TokenBalances.ContainsKey(request.ContractAddress) || sender.TokenBalances[request.ContractAddress] < request.Amount)
            return BadRequest(new { error = "Carteira de origem não possui saldo suficiente deste token." });

        var receiver = MongoDbService.Wallets.Find(w => w.Address == request.ToWallet).FirstOrDefault();
        if (receiver == null)
            return NotFound(new { error = "Carteira de destino não encontrada." });

        // Decrease from sender
        sender.TokenBalances[request.ContractAddress] -= request.Amount;
        
        // Add to receiver
        if (receiver.TokenBalances == null) receiver.TokenBalances = new System.Collections.Generic.Dictionary<string, decimal>();
        if (!receiver.TokenBalances.ContainsKey(request.ContractAddress))
            receiver.TokenBalances[request.ContractAddress] = 0;
            
        receiver.TokenBalances[request.ContractAddress] += request.Amount;

        MongoDbService.Wallets.ReplaceOne(w => w.InternalId == sender.InternalId, sender);
        MongoDbService.Wallets.ReplaceOne(w => w.InternalId == receiver.InternalId, receiver);

        return Ok(new { 
            message = "Transferência de token fracionado efetuada com sucesso.", 
            contractAddress = request.ContractAddress,
            from = request.FromWallet,
            to = request.ToWallet,
            amount = request.Amount
        });
    }
}

public class AssetTransferRequest
{
    public string ContractAddress { get; set; } = string.Empty;
    public string FromWallet { get; set; } = string.Empty;
    public string ToWallet { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class AssetMintToWalletRequest
{
    public string ContractAddress { get; set; } = string.Empty;
    public string TargetWallet { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class AssetCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal Supply { get; set; }
}

public class AssetMintRequest
{
    public string ContractAddress { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
