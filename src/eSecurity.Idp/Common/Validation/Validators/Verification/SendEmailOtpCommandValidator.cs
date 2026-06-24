using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Features.Verification.EmailOtp;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Verification;

public sealed class SendEmailOtpCommandValidator : AbstractValidator<SendEmailOtpCommand>
{
    public SendEmailOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("'email' is required")
            .EmailAddress().WithMessage("'email' is invalid");

        RuleFor(x => x.OperationType)
            .NotNull().WithMessage("'operation_type' is required")
            .NotEqual(OperationType.None).WithMessage("'operation_type' is invalid");
    }
}