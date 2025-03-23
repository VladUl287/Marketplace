namespace Product.Core.Events;

public sealed class ProductUpdatedEvent
{
    public string Topic { get; init; } = "product-updated";

    public Guid Id { get; init; }
}
