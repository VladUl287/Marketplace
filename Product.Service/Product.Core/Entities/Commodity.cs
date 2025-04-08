namespace Product.Core.Entities;

public sealed class Commodity
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
