using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class ChangeUserNameValidator : Validator<ChangeUserNameModel>
{
    public ChangeUserNameValidator()
    {
        RuleFor(model => model.UserName)
            .NotEmpty().WithMessage("Field is required.")
            .MaximumLength(32).WithMessage("Field must not exceed 32 characters.")
            .MinimumLength(3).WithMessage("Field must not exceed 3 characters.");
    }
}