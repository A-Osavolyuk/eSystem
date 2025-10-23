using eSystem.Application.Validation;
using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Validation;

public class RegistrationValidator : Validator<RegistrationRequest>
{
    public RegistrationValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Field is required.")
            .EmailAddress().WithMessage("Invalid format of email address.");
        
        RuleFor(p => p.Username)
            .NotEmpty().WithMessage("Field is required.")
            .MinimumLength(3).WithMessage("Field must be at least 3 characters long.")
            .MaximumLength(32).WithMessage("Field cannot exceed 32 characters.");

        RuleFor(p => p.Password).SetValidator(new PasswordValidator());
    }
}