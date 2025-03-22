namespace Product.Core.Events;

public sealed class ProductCreatedEvent
{
    public Guid Id { get; init; }
}
