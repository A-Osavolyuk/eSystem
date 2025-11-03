using eSystem.Core.Requests.Auth;
using eSystem.Core.Validation;

namespace eSecurity.Validation;

public class ForgotPasswordValidator : Validator<ForgotPasswordRequest>
{
    public ForgotPasswordValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Email is must.")
            .EmailAddress().WithMessage("Invalid format of email address.");
    }
}