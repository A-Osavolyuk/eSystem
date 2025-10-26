using eAccount.Blazor.Server.Domain.Models;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eAccount.Blazor.Server.Application.Validation;

public class AddPhoneNumberValidator : Validator<AddPhoneNumberModel>
{
    public AddPhoneNumberValidator()
    {
        RuleFor(model => model.PhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}