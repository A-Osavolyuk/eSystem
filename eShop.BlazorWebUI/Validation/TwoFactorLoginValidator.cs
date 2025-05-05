using eShop.BlazorWebUI.Models;
using eShop.Domain.Abstraction.Validation;
using FluentValidation;

namespace eShop.BlazorWebUI.Validation;

public class TwoFactorLoginValidator : Validator<TwoFactorLoginModel>
{
    public TwoFactorLoginValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code cannot be empty.")
            .Length(6).WithMessage("Must contain 6 characters.");
    }
}