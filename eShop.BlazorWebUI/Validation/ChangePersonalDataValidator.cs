using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class ChangePersonalDataValidator : Validator<ChangePersonalDataModel>
{
    public ChangePersonalDataValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Field is required!")
            .MinimumLength(3).WithMessage("Field length must be longer then 3 letters.")
            .MaximumLength(32).WithMessage("Field length must be less then 32 letters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Field is required!")
            .MinimumLength(3).WithMessage("Field length must be longer then 3 letters.")
            .MaximumLength(32).WithMessage("Field length must be less then 23 letters.");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Field is required!");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Now).WithMessage("You cannot choose today`s date.");
    }
}