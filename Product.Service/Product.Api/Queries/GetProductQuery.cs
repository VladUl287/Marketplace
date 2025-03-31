using Product.Api.Abstractions.Request;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Product.Api.Queries;

[ImmutableObject(true)]
public sealed class GetProductQuery : Cacheable
{
    public Guid Id { get; init; }

    public override ImmutableArray<string> Tags => _tags;
    public override string BuildKey() => $"get-product-{Id}";


    private static readonly ImmutableArray<string> _tags = ["products"];
}
