using System;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Aethos.Presentation.RPC.Data;

public static class MongoDbService
{
    private static readonly Lazy<MongoClient> LazyClient = new Lazy<MongoClient>(() => new MongoClient("mongodb://mplopes:3702959@localhost:27017/"));

    public static MongoClient Client => LazyClient.Value;
    public static IMongoDatabase Database => Client.GetDatabase("AethosLedger");
    public static IMongoCollection<WalletData> Wallets => Database.GetCollection<WalletData>("Wallets");
    public static IMongoCollection<AssetData> Assets => Database.GetCollection<AssetData>("Assets");
    public static IMongoCollection<NftData> Nfts => Database.GetCollection<NftData>("Nfts");
}

public class WalletData
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? InternalId { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Balance { get; set; } = "0";
    public string Currency { get; set; } = "DTC";
    public System.Collections.Generic.Dictionary<string, decimal> TokenBalances { get; set; } = new();
    public System.Collections.Generic.Dictionary<string, int> NftBalances { get; set; } = new();
}


public class AssetData
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? InternalId { get; set; }
    public string ContractAddress { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal MaxSupply { get; set; }
    public decimal CurrentMinted { get; set; }
}

public class NftData
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? InternalId { get; set; }
    public string TokenId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MetadataUri { get; set; } = string.Empty;
    public string CreatorAddress { get; set; } = string.Empty;
    public int MaxSupply { get; set; }
    public int CurrentMinted { get; set; }
}
