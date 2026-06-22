using eSecurity.Idp.Features.Email.Verification;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Email;

public sealed class SendEmailVerificationOtpCommandValidator : AbstractValidator<SendEmailVerificationOtpCommand>
{
    public SendEmailVerificationOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("'email' is required")
            .EmailAddress().WithMessage("'email' is invalid");
    }
}