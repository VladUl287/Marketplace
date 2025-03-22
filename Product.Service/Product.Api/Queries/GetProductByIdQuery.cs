using MediatR;
using Product.Api.DTOs;

namespace Product.Api.Queries;

public sealed class GetProductByIdQuery(Guid id) : IRequest<ProductDTO>
{
    public Guid Id => id;
}
