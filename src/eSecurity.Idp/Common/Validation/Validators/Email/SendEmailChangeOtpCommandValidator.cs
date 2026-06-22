using eSecurity.Idp.Common.Validation.Validators.Standard;
using eSecurity.Idp.Features.Email.Change;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Email;

public sealed class SendEmailChangeOtpCommandValidator : AbstractValidator<SendEmailChangeOtpCommand>
{
    public SendEmailChangeOtpCommandValidator()
    {
        RuleFor(x => x.NewEmail)
            .SetValidator(new EmailValidator("new_email"));
    }
}