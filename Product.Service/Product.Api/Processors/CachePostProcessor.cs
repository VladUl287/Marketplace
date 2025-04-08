using FastEndpoints;
using Microsoft.Extensions.Caching.Hybrid;
using Product.Api.Abstractions.Request;

namespace Product.Api.Processors;

public sealed class CachePostProcessor<TRequest, TResponse>(HybridCache hybridCache) : PostProcessor<TRequest, CacheBag, TResponse>
    where TRequest : Cacheable
{
    public override async Task PostProcessAsync(IPostProcessorContext<TRequest, TResponse> context, CacheBag state, CancellationToken ct)
    {
        var statusCode = context.HttpContext.Response.StatusCode;
        if (statusCode < 200 || statusCode >= 300 || state.IsCached)
        {
            return;
        }

        var request = context.Request ?? throw new NullReferenceException();
        await hybridCache.SetAsync(
            request.GetKey(),
            context.Response,
            null,
            tags: request.Tags,
            cancellationToken: ct);
    }
}
