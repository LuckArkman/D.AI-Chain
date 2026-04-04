namespace Aethos.Domain.ValueObjects;

public readonly record struct ResultHash
{
    public string Value { get; }

    private ResultHash(string value)
    {
        Value = value;
    }

    public static ResultHash Create(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Hash cannot be empty.", nameof(hash));

        if (!hash.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Hash must start with '0x'.", nameof(hash));

        if (hash.Length != 66)
            throw new ArgumentException("Result hash must be exactly 66 characters long.", nameof(hash));

        var span = hash.AsSpan(2);
        foreach (var c in span)
        {
            if (!IsHex(c))
                throw new ArgumentException($"Invalid character '{c}' in hash.", nameof(hash));
        }

        return new ResultHash(hash.ToLowerInvariant());
    }

    private static bool IsHex(char c) => 
        (c >= '0' && c <= '9') || 
        (c >= 'a' && c <= 'f') || 
        (c >= 'A' && c <= 'F');

    public override string ToString() => Value;
}
