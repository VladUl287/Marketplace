using MediatR;
using Product.Api.DTOs;

namespace Product.Api.Commands;

public sealed class CreateProductRequest : IRequest<ProductDTO>
{
}
