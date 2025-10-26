using eAccount.Domain.Models;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eAccount.Application.Validation;

public class AddPhoneNumberValidator : Validator<AddPhoneNumberModel>
{
    public AddPhoneNumberValidator()
    {
        RuleFor(model => model.PhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}