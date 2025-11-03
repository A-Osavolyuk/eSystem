using eSystem.Core.Requests.Auth;
using eSystem.Core.Validation;

namespace eSecurity.Validation;

public class ResetEmailValidator : Validator<ResetEmailRequest>
{
    public ResetEmailValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("New email is must!")
            .EmailAddress().WithMessage("Invalid email address format.");
    }
}