using System;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Aethos.Presentation.RPC.Data;
using System.Linq;

namespace Aethos.Presentation.RPC.Controllers;

[ApiController]
[Route("api/nft")]
public class NftController : ControllerBase
{
    [HttpPost("create")]
    public IActionResult CreateNft([FromBody] NftCreateRequest request)
    {
        if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.MetadataUri) || request.Supply <= 0)
            return BadRequest(new { error = "O nome, a MetadataUri e o Supply (maior que 0) são obrigatórios." });

        var tokenId = "0xNFT" + Guid.NewGuid().ToString("N");

        var nftData = new NftData
        {
            TokenId = tokenId,
            Name = request.Name,
            Description = request.Description ?? "",
            MetadataUri = request.MetadataUri,
            CreatorAddress = request.CreatorAddress ?? "0x0",
            MaxSupply = request.Supply,
            CurrentMinted = 0
        };

        MongoDbService.Nfts.InsertOne(nftData);

        return Ok(new { 
            message = "Tipo de NFT criado com sucesso na rede.", 
            tokenId = nftData.TokenId,
            name = nftData.Name,
            maxSupply = nftData.MaxSupply,
            creator = nftData.CreatorAddress
        });
    }

    [HttpPost("mint-to-wallet")]
    public IActionResult MintToWallet([FromBody] NftMintRequest request)
    {
        if (string.IsNullOrEmpty(request.TokenId) || string.IsNullOrEmpty(request.TargetWallet))
            return BadRequest(new { error = "TokenId e TargetWallet são obrigatórios." });

        int amountToMint = request.Amount <= 0 ? 1 : request.Amount;

        var nft = MongoDbService.Nfts.Find(n => n.TokenId == request.TokenId).FirstOrDefault();
        if (nft == null)
            return NotFound(new { error = "NFT Tipo (Contract) não encontrado na rede." });

        if (nft.CurrentMinted + amountToMint > nft.MaxSupply)
            return BadRequest(new { error = "O limite de supply para este NFT foi excedido." });

        var wallet = MongoDbService.Wallets.Find(w => w.Address == request.TargetWallet).FirstOrDefault();
        if (wallet == null)
            return NotFound(new { error = "Carteira de destino não encontrada." });

        nft.CurrentMinted += amountToMint;
        
        if (wallet.NftBalances == null) wallet.NftBalances = new System.Collections.Generic.Dictionary<string, int>();
        if (!wallet.NftBalances.ContainsKey(nft.TokenId))
            wallet.NftBalances[nft.TokenId] = 0;
            
        wallet.NftBalances[nft.TokenId] += amountToMint;

        MongoDbService.Nfts.ReplaceOne(n => n.InternalId == nft.InternalId, nft);
        MongoDbService.Wallets.ReplaceOne(w => w.InternalId == wallet.InternalId, wallet);

        return Ok(new { 
            message = "NFTs creditado(s) à carteira com sucesso.", 
            tokenId = nft.TokenId,
            targetWallet = wallet.Address,
            amountMinted = amountToMint,
            walletTotalForNft = wallet.NftBalances[nft.TokenId]
        });
    }

    [HttpPost("transfer")]
    public IActionResult TransferNft([FromBody] NftTransferRequest request)
    {
        if (string.IsNullOrEmpty(request.TokenId) || string.IsNullOrEmpty(request.FromWallet) || string.IsNullOrEmpty(request.ToWallet))
            return BadRequest(new { error = "TokenId, FromWallet e ToWallet são obrigatórios." });

        int amountToTransfer = request.Amount <= 0 ? 1 : request.Amount;

        var sender = MongoDbService.Wallets.Find(w => w.Address == request.FromWallet).FirstOrDefault();
        if (sender == null || sender.NftBalances == null || !sender.NftBalances.ContainsKey(request.TokenId) || sender.NftBalances[request.TokenId] < amountToTransfer)
            return BadRequest(new { error = "A carteira de origem não possui unidades suficientes deste NFT para transferir." });

        var receiver = MongoDbService.Wallets.Find(w => w.Address == request.ToWallet).FirstOrDefault();
        if (receiver == null)
            return NotFound(new { error = "Carteira de destino não encontrada." });

        // Debit from sender
        sender.NftBalances[request.TokenId] -= amountToTransfer;

        // Credit to receiver
        if (receiver.NftBalances == null) receiver.NftBalances = new System.Collections.Generic.Dictionary<string, int>();
        if (!receiver.NftBalances.ContainsKey(request.TokenId))
            receiver.NftBalances[request.TokenId] = 0;
            
        receiver.NftBalances[request.TokenId] += amountToTransfer;

        MongoDbService.Wallets.ReplaceOne(w => w.InternalId == sender.InternalId, sender);
        MongoDbService.Wallets.ReplaceOne(w => w.InternalId == receiver.InternalId, receiver);

        return Ok(new { 
            message = "Transferência de NFT efetuada com sucesso.", 
            tokenId = request.TokenId,
            from = request.FromWallet,
            to = request.ToWallet,
            amountTransferred = amountToTransfer
        });
    }

    [HttpGet("wallet/{address}")]
    public IActionResult GetWalletNfts(string address)
    {
        if (string.IsNullOrEmpty(address))
            return BadRequest(new { error = "O endereço da carteira é obrigatório." });

        var wallet = MongoDbService.Wallets.Find(w => w.Address == address).FirstOrDefault();
        if (wallet == null) return NotFound(new { error = "Carteira não encontrada." });

        var balances = wallet.NftBalances ?? new System.Collections.Generic.Dictionary<string, int>();

        var nftsData = balances.Where(kv => kv.Value > 0).Select(kv => {
            var nftType = MongoDbService.Nfts.Find(n => n.TokenId == kv.Key).FirstOrDefault();
            return new {
                tokenId = kv.Key,
                name = nftType?.Name ?? "Unknown",
                description = nftType?.Description ?? "",
                metadataUri = nftType?.MetadataUri ?? "",
                amountOwned = kv.Value
            };
        });

        return Ok(new { 
            address = address,
            ownedNftTypesCount = nftsData.Count(),
            nfts = nftsData
        });
    }
}

public class NftCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string MetadataUri { get; set; } = string.Empty;
    public string? CreatorAddress { get; set; }
    public int Supply { get; set; }
}

public class NftMintRequest
{
    public string TokenId { get; set; } = string.Empty;
    public string TargetWallet { get; set; } = string.Empty;
    public int Amount { get; set; } = 1;
}

public class NftTransferRequest
{
    public string TokenId { get; set; } = string.Empty;
    public string FromWallet { get; set; } = string.Empty;
    public string ToWallet { get; set; } = string.Empty;
    public int Amount { get; set; } = 1;
}
