namespace Product.Core.Events;

public sealed class ProductCreatedEvent
{
    public string Topic { get; init; } = "product-created";

    public Guid Id { get; init; }
}
