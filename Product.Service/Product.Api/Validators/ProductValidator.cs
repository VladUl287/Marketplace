using FluentValidation;
using Product.Api.DTOs;

namespace Product.Api.Validators;

public sealed class ProductValidator : AbstractValidator<ProductDTO>
{
    public ProductValidator()
    {
    }
}
