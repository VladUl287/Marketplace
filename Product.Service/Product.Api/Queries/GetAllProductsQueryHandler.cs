using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Api.DTOs;
using Product.Infrastructure.Data;

namespace Product.Api.Queries;

public sealed class GetAllProductsQueryHandler(ProductsDbContext productsDbContext) : IRequestHandler<GetAllProductsQuery, ProductDTO[]>
{
    public async Task<ProductDTO[]> Handle(GetAllProductsQuery _request, CancellationToken cancellationToken)
    {
        var products = await productsDbContext.Products
            .ProjectToType<ProductDTO>()
            .ToArrayAsync(cancellationToken);
        return products;
    }
}
