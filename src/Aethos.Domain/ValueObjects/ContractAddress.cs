namespace Aethos.Domain.ValueObjects;

public readonly record struct ContractAddress
{
    public string Value { get; }

    private ContractAddress(string value)
    {
        Value = value;
    }

    public static ContractAddress Create(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be empty.", nameof(address));

        if (!address.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Address must start with '0x'.", nameof(address));

        if (address.Length != 42)
            throw new ArgumentException("Address must be exactly 42 characters long.", nameof(address));

        var span = address.AsSpan(2);
        foreach (var c in span)
        {
            if (!IsHex(c))
                throw new ArgumentException($"Invalid character '{c}' in address.", nameof(address));
        }

        return new ContractAddress(address.ToLowerInvariant());
    }

    private static bool IsHex(char c) => 
        (c >= '0' && c <= '9') || 
        (c >= 'a' && c <= 'f') || 
        (c >= 'A' && c <= 'F');

    public override string ToString() => Value;
}
