using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class VerifyEmailValidator : Validator<VerifyEmailModel>
{
    public VerifyEmailValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .Length(6).WithMessage("Code length must be 6 characters.")
            .Matches(@"^\d+$").WithMessage("Code must be numeric.");
    }
}