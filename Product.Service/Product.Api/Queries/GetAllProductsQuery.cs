using MediatR;
using Product.Api.DTOs;

namespace Product.Api.Queries;

public sealed class GetAllProductsQuery : IRequest<ProductDTO[]> { }
