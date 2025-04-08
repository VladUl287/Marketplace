using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Product.Api.DTOs;
using Product.Api.Processors;
using Product.Infrastructure.Data;

namespace Product.Api.Queries;

public sealed class GetProductEndpoint(ProductsDbContext context) : Endpoint<GetProductQuery, ProductDTO>
{
    public override void Configure()
    {
        Get("/product/get");
        PreProcessor<CachePreProcessor<GetProductQuery, ProductDTO>>();
        PostProcessor<CachePostProcessor<GetProductQuery, ProductDTO>>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetProductQuery request, CancellationToken token)
    {
        var product = await GetProductById(context, request.Id);
        if (product is null)
        {
            await SendNotFoundAsync(token);
            return;
        }
        await SendOkAsync(product, token);
    }

    private static readonly Func<ProductsDbContext, Guid, Task<ProductDTO?>> GetProductById =
       EF.CompileAsyncQuery((ProductsDbContext context, Guid id) =>
           context.Products
               .ProjectToType<ProductDTO>(null)
               .FirstOrDefault(c => c.Id == id)
       );
}