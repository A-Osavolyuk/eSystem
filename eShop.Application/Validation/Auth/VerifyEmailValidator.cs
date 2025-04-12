using eShop.Domain.Requests.API.Auth;

namespace eShop.Application.Validation.Auth;

public class VerifyEmailValidator : Validator<VerifyEmailRequest>
{
    public VerifyEmailValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .Length(6).WithMessage("Code length must be 6 characters.")
            .Matches(@"^\d+$").WithMessage("Code must be numeric.");
    }
}