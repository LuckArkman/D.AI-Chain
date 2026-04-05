namespace Aethos.Domain.ValueObjects;

public readonly record struct BlockHash
{
    public string Value { get; }

    private BlockHash(string value) => Value = value;

    public static BlockHash Create(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Hash cannot be empty.");

        if (!hash.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Hash must start with '0x'.");

        if (hash.Length != 66)
            throw new ArgumentException("Block hash must be exactly 66 characters long.");

        return new BlockHash(hash.ToLowerInvariant());
    }

    public override string ToString() => Value;
}
