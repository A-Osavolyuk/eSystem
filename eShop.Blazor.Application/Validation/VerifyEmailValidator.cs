namespace eShop.Blazor.Application.Validation;

public class VerifyEmailValidator : Validator<VerifyEmailModel>
{
    public VerifyEmailValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}