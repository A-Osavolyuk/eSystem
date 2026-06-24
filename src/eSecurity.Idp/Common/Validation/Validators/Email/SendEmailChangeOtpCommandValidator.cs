using eSecurity.Idp.Features.Email.Change;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Email;

public sealed class SendEmailChangeOtpCommandValidator : AbstractValidator<SendEmailChangeOtpCommand>
{
    public SendEmailChangeOtpCommandValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("'new_email' is required")
            .EmailAddress().WithMessage("'new_email' is invalid");
    }
}