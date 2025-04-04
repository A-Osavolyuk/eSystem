using eShop.Domain.Requests.Api.Auth;

namespace eShop.Application.Validation.Auth;

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
            .NotEmpty().WithMessage("Gender is must!")
            .MinimumLength(3).WithMessage("Gender must length be longer then 3 letters.")
            .MaximumLength(32).WithMessage("Gender must length be less then 32 letters.");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Now).WithMessage("You cannot choose today`s date.");
    }
}