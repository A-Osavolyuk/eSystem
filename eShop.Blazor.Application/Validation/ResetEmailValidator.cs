namespace eShop.Blazor.Application.Validation;

public class ResetEmailValidator : Validator<ResetEmailModel>
{
    public ResetEmailValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("New email is must!")
            .EmailAddress().WithMessage("Invalid email address format.");
    }
}