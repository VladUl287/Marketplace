namespace Product.Core.ValueObjects;

public sealed class Price
{
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
}