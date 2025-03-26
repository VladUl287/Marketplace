using MediatR;
using Product.Api.DTOs;

namespace Product.Api.Queries;

public sealed class GetProductQuery(Guid id) : IRequest<ProductDTO>
{
    public Guid Id => id;
}
