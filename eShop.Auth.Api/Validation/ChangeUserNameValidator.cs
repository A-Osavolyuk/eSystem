using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Validation;

public class ChangeUserNameValidator : Validator<ChangeUsernameRequest>
{
    public ChangeUserNameValidator()
    {
        RuleFor(model => model.Username)
            .NotEmpty().WithMessage("Field is required.")
            .MaximumLength(32).WithMessage("Field must not exceed 32 characters.")
            .MinimumLength(3).WithMessage("Field must not exceed 3 characters.");
    }
}