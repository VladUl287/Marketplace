using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Product.Api.DTOs;
using Product.Infrastructure.Data;

namespace Product.Api.Queries;

public sealed class GetProductEndpoint(ProductsDbContext productsDbContext) : Endpoint<GetProductQuery, ProductDTO?>
{
    public override void Configure()
    {
        Get("/product/get");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetProductQuery request, CancellationToken token)
    {
        var product = await productsDbContext.Products
            .Where(c => c.Id == request.Id)
            .ProjectToType<ProductDTO>()
            .FirstOrDefaultAsync(token);

        await SendAsync(product, cancellation: token);
    }
}