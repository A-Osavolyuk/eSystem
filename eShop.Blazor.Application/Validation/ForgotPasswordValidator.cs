namespace eShop.Blazor.Application.Validation;

public class ForgotPasswordValidator : Validator<ForgotPasswordModel>
{
    public ForgotPasswordValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Email is must.")
            .EmailAddress().WithMessage("Invalid format of email address.");
    }
}