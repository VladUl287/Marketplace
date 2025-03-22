namespace Product.Core.Entities;

public sealed class Outbox
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public required string Payload { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
