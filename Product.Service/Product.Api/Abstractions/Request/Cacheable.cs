using System.Collections.Immutable;

namespace Product.Api.Abstractions.Request;

public abstract class Cacheable
{
    public abstract string GetKey();

    public virtual ImmutableArray<string> Tags => [];
}
