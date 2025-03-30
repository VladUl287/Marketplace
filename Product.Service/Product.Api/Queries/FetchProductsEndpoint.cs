using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Product.Api.DTOs;
using Product.Infrastructure.Data;

namespace Product.Api.Queries;

//public sealed class FetchProductsEndpoint(ProductsDbContext productsDbContext) : Endpoint<FetchProductsQuery, ProductDTO[]>
//{
//    public override void Configure()
//    {
//        Get("/product/fetch");
//        AllowAnonymous();
//    }

//    public override async Task HandleAsync(FetchProductsQuery _request, CancellationToken token)
//    {
//        var products = await productsDbContext.Products
//            .ProjectToType<ProductDTO>()
//            .ToArrayAsync(token);

//        await SendAsync(products, cancellation: token);
//    }
//}
