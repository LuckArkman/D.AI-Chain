namespace Aethos.Domain.ValueObjects;

public readonly record struct ContractAddress
{
    public string Value { get; }

    private ContractAddress(string value) => Value = value;

    public static ContractAddress Create(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be empty.");

        if (!address.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Address must start with '0x'.");

        if (address.Length != 42)
            throw new ArgumentException("Address must be exactly 42 characters long.");

        string hexPart = address.Substring(2);
        if (!System.Text.RegularExpressions.Regex.IsMatch(hexPart, @"\b[0-9a-fA-F]+\b"))
            throw new ArgumentException("Address must contain only hexadecimal characters.");

        return new ContractAddress(address.ToLowerInvariant());
    }

    public override string ToString() => Value;
}
