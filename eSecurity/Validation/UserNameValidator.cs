using eSystem.Core.Requests.Auth;
using eSystem.Core.Validation;

namespace eSecurity.Validation;

public class UserNameValidator : Validator<ChangeUsernameRequest>
{
    public UserNameValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("User name cannot be empty.")
            .MinimumLength(3).WithMessage("User name must be at least 3 characters long")
            .MaximumLength(18).WithMessage("User name cannot be longer then 18 characters.");
    }
}