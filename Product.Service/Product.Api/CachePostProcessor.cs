using FastEndpoints;
using Microsoft.Extensions.Caching.Hybrid;
using Product.Api.Abstractions.Request;

namespace Product.Api;

public sealed class CachePostProcessor<TRequest, TResponse>(HybridCache hybridCache) : PostProcessor<TRequest, CacheBag, TResponse> 
    where TRequest : Cacheable
{
    public override async Task PostProcessAsync(IPostProcessorContext<TRequest, TResponse> context, CacheBag state, CancellationToken ct)
    {
        if (state.IsCached)
        {
            return;
        }

        var request = context.Request ?? throw new NullReferenceException();
        await hybridCache.SetAsync(
            request.BuildKey(),
            context.Response,
            new()
            {
                Expiration = TimeSpan.FromHours(3)
            },
            tags: request.Tags,
            cancellationToken: ct);
    }
}
