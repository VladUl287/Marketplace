using FastEndpoints;
using Microsoft.Extensions.Caching.Hybrid;
using Product.Api.Abstractions.Request;

namespace Product.Api;

public sealed class CachePostProcessorGlobal(HybridCache hybridCache) : IGlobalPostProcessor
{
    public async Task PostProcessAsync(IPostProcessorContext context, CancellationToken token)
    {
        if (context.Request is Cacheable cacheable)
        {
            await hybridCache.SetAsync(
                cacheable.BuildKey(),
                context.Response,
                tags: cacheable.Tags,
                cancellationToken: token);
        }
    }
}
