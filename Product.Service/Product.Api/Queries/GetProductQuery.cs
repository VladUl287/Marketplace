using Product.Api.Abstractions.Request;
using System.Collections.Immutable;

namespace Product.Api.Queries;

public sealed class GetProductQuery : Cacheable
{
    public Guid Id { get; init; }

    public override string GetKey() => string.Intern($"get_product_{Id}");

    private static readonly ImmutableArray<string> _tags = ["products"];
    public override ImmutableArray<string> Tags => _tags;
}
