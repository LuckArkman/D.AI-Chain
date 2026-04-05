namespace Aethos.Domain.ValueObjects;

public readonly record struct ResultHash
{
    public string Value { get; }

    private ResultHash(string value) => Value = value;

    public static ResultHash Create(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Result hash cannot be empty.");

        if (!hash.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Result hash must start with '0x'.");

        if (hash.Length != 66)
            throw new ArgumentException("Result hash must be exactly 66 characters long.");

        return new ResultHash(hash.ToLowerInvariant());
    }

    public override string ToString() => Value;
}
