using eSecurity.Idp.Features.Email.Reset;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Email;

public sealed class ResetEmailCommandValidator : AbstractValidator<ResetEmailCommand>
{
    public ResetEmailCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("'code' is required");
        
        RuleFor(x => x.CurrentEmail)
            .NotEmpty().WithMessage("'current_email' is required")
            .EmailAddress().WithMessage("'current_email' is invalid");
        
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("'new_email' is required")
            .EmailAddress().WithMessage("'new_email' is invalid");
    }
}