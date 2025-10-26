using eSystem.Core.Requests.Auth;
using eSystem.Core.Validation;

namespace eSystem.Auth.Api.Validation;

public class PersonalDataValidator : Validator<ChangePersonalDataRequest>
{
    public PersonalDataValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First Name is must!")
            .MinimumLength(3).WithMessage("First Name length must be longer then 3 letters.")
            .MaximumLength(32).WithMessage("First Name length must be less then 32 letters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("First Name is must!")
            .MinimumLength(3).WithMessage("Last Name length must be longer then 3 letters.")
            .MaximumLength(32).WithMessage("Last Name length must be less then 23 letters.");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is must!");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Now).WithMessage("You cannot choose today`s date.");
    }
}