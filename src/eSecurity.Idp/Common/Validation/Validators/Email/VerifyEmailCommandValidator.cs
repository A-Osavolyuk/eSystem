using eSecurity.Idp.Common.Validation.Validators.Standard;
using eSecurity.Idp.Features.Email.Verification;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Email;

public sealed class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("'code' is required");

        RuleFor(x => x.Email)
            .SetValidator(new EmailValidator("email"));
    }
}