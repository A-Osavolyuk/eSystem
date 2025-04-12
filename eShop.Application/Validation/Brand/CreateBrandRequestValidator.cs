using eShop.Domain.Requests.API.Brand;

namespace eShop.Application.Validation.Brand;

public class CreateBrandRequestValidator : Validator<CreateBrandRequest>
{
    public CreateBrandRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long")
            .MaximumLength(32).WithMessage("Name must not be longer than 32 characters");
        
        RuleFor(x => x.Country).NotEmpty().WithMessage("Country is required");
    }
}