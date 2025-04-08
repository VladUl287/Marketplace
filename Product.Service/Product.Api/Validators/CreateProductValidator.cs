using FluentValidation;
using Product.Api.Commands;

namespace Product.Api.Validators;

public sealed class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .WithMessage("Name is required");
    }
}
