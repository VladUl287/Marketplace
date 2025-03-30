using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Product.Api.DTOs;
using Product.Infrastructure.Data;

namespace Product.Api.Queries;

public sealed class GetProductEndpoint(
    //ProductsDbContext productsDbContext,
    HybridCache cache
    ) : Endpoint<GetProductQuery, ProductDTO?>
{
    public override void Configure()
    {
        Get("/product/get");
        AllowAnonymous();
    }

    private static int Time = 1;

    public override async Task HandleAsync(GetProductQuery request, CancellationToken cancellationToken)
    {
        if (Time % 2 == 0)
        {
            await cache.RemoveByTagAsync("product", cancellationToken);
        }
        Time++;

        var product = await cache.GetOrCreateAsync(
                $"product-{request.Id}",
                request,
                async (state, cancel) =>
                {
                    //return await productsDbContext.Products
                    //    .ProjectToType<ProductDTO>()
                    //    .FirstOrDefaultAsync(c => c.Id == state.Id);
                    return new ProductDTO
                    {
                        Id = Guid.NewGuid()
                    };
                },
                options: new()
                {
                    Expiration = TimeSpan.FromHours(3)
                },
                tags: ["product"],
                cancellationToken: cancellationToken
            );

        await SendAsync(product, cancellation: cancellationToken);
    }
}