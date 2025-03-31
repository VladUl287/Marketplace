using System.Collections.Immutable;

namespace Product.Api.Abstractions.Request;


public abstract class Cacheable
{
    public virtual ImmutableArray<string> Tags => [];

    public abstract string BuildKey();
}
