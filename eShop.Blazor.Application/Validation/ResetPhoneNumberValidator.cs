namespace eShop.Blazor.Application.Validation;

public class ResetPhoneNumberValidator : Validator<ResetPhoneNumberModel>
{
    public ResetPhoneNumberValidator()
    {
        RuleFor(x => x.NewPhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}