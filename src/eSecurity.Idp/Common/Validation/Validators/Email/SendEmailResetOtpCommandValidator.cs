using eSecurity.Idp.Common.Validation.Validators.Standard;
using eSecurity.Idp.Features.Email.Reset;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Email;

public sealed class SendEmailResetOtpCommandValidator : AbstractValidator<SendEmailResetOtpCommand>
{
    public SendEmailResetOtpCommandValidator()
    {
        RuleFor(x => x.CurrentEmail)
            .SetValidator(new EmailValidator("current_email"));
        
        RuleFor(x => x.NewEmail)
            .SetValidator(new EmailValidator("new_email"));
    }
}