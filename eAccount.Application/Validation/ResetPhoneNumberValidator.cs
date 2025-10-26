using eAccount.Domain.Models;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eAccount.Application.Validation;

public class ResetPhoneNumberValidator : Validator<ResetPhoneNumberModel>
{
    public ResetPhoneNumberValidator()
    {
        RuleFor(x => x.NewPhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}