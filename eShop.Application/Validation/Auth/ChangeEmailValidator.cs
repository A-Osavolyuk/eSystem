using eShop.Domain.Requests.Api.Auth;

namespace eShop.Application.Validation.Auth;

public class ChangeEmailValidator : Validator<ChangeEmailRequest>
{
    public ChangeEmailValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("New email is must!")
            .EmailAddress().WithMessage("Invalid email address format.");
    }
}