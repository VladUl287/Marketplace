using FastEndpoints;
using Microsoft.Extensions.Caching.Hybrid;
using Product.Api.Abstractions.Request;

namespace Product.Api.Processors;

public sealed class CachePreProcessor<TRequest, TResponse>(HybridCache hybridCache) : PreProcessor<TRequest, CacheBag> 
    where TRequest : Cacheable
{
    private static readonly HybridCacheEntryOptions _options = new()
    {
        Flags = HybridCacheEntryFlags.DisableUnderlyingData
    };

    public override async Task PreProcessAsync(IPreProcessorContext<TRequest> context, CacheBag state, CancellationToken ct)
    {
        var request = context.Request ?? throw new NullReferenceException();

        var cachedValue = await hybridCache.GetOrCreateAsync<TResponse>(
            request.GetKey(),
            factory: null,
            _options,
            request.Tags,
            ct);

        if (cachedValue is not null)
        {
            state.IsCached = true;
            await context.HttpContext.Response.SendAsync(cachedValue, cancellation: ct);
        }
    }
}
