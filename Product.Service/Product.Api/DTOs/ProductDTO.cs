using System.ComponentModel;

namespace Product.Api.DTOs;

[ImmutableObject(true)]
public sealed class ProductDTO
{
    public Guid Id { get; init; }

    public string? Name { get; init; }
}
