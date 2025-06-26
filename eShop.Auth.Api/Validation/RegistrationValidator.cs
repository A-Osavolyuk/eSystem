using eShop.Application.Validation;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Validation;

public class RegistrationValidator : Validator<RegistrationRequest>
{
    public RegistrationValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Email is must.")
            .EmailAddress().WithMessage("Invalid format of email address.");

        RuleFor(p => p.Password).SetValidator(new PasswordValidator());

        RuleFor(p => p.ConfirmPassword)
            .NotEmpty().WithMessage("You must confirm your password.")
            .Equal(x => x.Password).WithMessage("Must be the same with password.");

    }
}