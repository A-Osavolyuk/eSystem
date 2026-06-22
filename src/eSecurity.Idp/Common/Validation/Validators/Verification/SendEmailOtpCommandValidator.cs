using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Common.Validation.Validators.Standard;
using eSecurity.Idp.Features.Verification.EmailOtp;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Verification;

public sealed class SendEmailOtpCommandValidator : AbstractValidator<SendEmailOtpCommand>
{
    public SendEmailOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .SetValidator(new EmailValidator("email"));

        RuleFor(x => x.OperationType)
            .NotNull().WithMessage("'operation_type' is required")
            .NotEqual(OperationType.None).WithMessage("'operation_type' is invalid");
    }
}