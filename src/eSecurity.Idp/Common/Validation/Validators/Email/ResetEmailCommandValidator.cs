using eSecurity.Idp.Common.Validation.Validators.Standard;
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
            .SetValidator(new EmailValidator("current_email"));
        
        RuleFor(x => x.NewEmail)
            .SetValidator(new EmailValidator("new_email"));
    }
}