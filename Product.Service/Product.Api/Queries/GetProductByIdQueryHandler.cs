using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Api.DTOs;
using Product.Infrastructure.Data;

namespace Product.Api.Queries;

public sealed class GetProductByIdQueryHandler(ProductsDbContext productsDbContext) : IRequestHandler<GetProductByIdQuery, ProductDTO?>
{
    public Task<ProductDTO?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return productsDbContext.Products
            .Where(c => c.Id == request.Id)
            .ProjectToType<ProductDTO>()
            .FirstOrDefaultAsync(cancellationToken);
    }
}