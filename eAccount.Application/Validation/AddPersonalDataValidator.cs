using eAccount.Domain.Models;
using eSystem.Core.Validation;

namespace eAccount.Application.Validation;

public class AddPersonalDataValidator : Validator<AddPersonalDataModel>
{
    public AddPersonalDataValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Field is required!")
            .MinimumLength(3).WithMessage("Field length must be longer then 3 letters.")
            .MaximumLength(64).WithMessage("Field length must be less then 64 letters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Field is required!")
            .MinimumLength(3).WithMessage("Field length must be longer then 3 letters.")
            .MaximumLength(64).WithMessage("Field length must be less then 64 letters.");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Field is required!");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Now).WithMessage("You cannot choose today`s date.");
        
        When(x => !string.IsNullOrEmpty(x.MiddleName), () =>
        {
            RuleFor(x => x.MiddleName)
                .MinimumLength(3).WithMessage("Field length must be longer then 3 letters.")
                .MaximumLength(64).WithMessage("Field length must be less then 64 letters.");
        });
    }
}