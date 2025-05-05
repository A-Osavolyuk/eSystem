using eShop.Domain.Requests.API.Auth;

namespace eShop.BlazorWebUI.Validation;

public class ChangeEmailValidator : Validator<ChangeEmailRequest>
{
    public ChangeEmailValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("New email is must!")
            .EmailAddress().WithMessage("Invalid email address format.");
    }
}