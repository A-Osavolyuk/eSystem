using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Validation;

public class TwoFactorLoginValidator : Validator<LoginWith2FaRequest>
{
    public TwoFactorLoginValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Code cannot be empty.")
            .Length(6).WithMessage("Must contain 6 characters.");
    }
}