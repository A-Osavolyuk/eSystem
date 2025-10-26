using eAccount.Domain.Models;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eAccount.Application.Validation;

public class ChangePhoneNumberValidator : Validator<ChangePhoneNumberModel>
{
    public ChangePhoneNumberValidator()
    {
        RuleFor(x => x.NewPhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}