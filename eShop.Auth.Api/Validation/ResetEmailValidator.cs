using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Validation;

public class ResetEmailValidator : Validator<ResetEmailRequest>
{
    public ResetEmailValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("New email is must!")
            .EmailAddress().WithMessage("Invalid email address format.");
    }
}