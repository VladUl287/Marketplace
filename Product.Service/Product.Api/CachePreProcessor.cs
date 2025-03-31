using FastEndpoints;
using Microsoft.Extensions.Caching.Hybrid;
using Product.Api.Abstractions.Request;

namespace Product.Api;

public sealed class CachePreProcessor(HybridCache hybridCache, IServiceProvider _serviceProvider) : IGlobalPreProcessor
{
    public async Task PreProcessAsync(IPreProcessorContext context, CancellationToken token)
    {
        if (context.Request is Cacheable cacheable)
        {
            var cachedValue = await hybridCache.GetOrCreateAsync<object?>(
                cacheable.BuildKey(),
                factory: null,
                new()
                {
                    Flags = HybridCacheEntryFlags.DisableUnderlyingData
                },
                cacheable.Tags,
                token);

            if (cachedValue is not null)
            {
                await context.HttpContext.Response.SendAsync(cachedValue, cancellation: token);
            }
        }
    }
}
