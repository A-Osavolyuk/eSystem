using eSecurity.Idp.Features.Email.Change;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Email;

public sealed class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
{
    public ChangeEmailCommandValidator()
    {
        RuleFor(x => x.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("'code' is required");
        
        RuleFor(x => x.CurrentEmail)
            .NotEmpty().WithMessage("'current_email' is required")
            .EmailAddress().WithMessage("'current_email' is invalid");

        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("'new_email' is required")
            .EmailAddress().WithMessage("'new_email' is invalid");
    }
}