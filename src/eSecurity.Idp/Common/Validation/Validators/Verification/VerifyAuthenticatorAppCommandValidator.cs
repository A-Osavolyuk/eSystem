using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Features.Verification.AuthenticatorApp;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Verification;

public sealed class VerifyAuthenticatorAppCommandValidator : AbstractValidator<VerifyAuthenticatorAppCommand>
{
    public VerifyAuthenticatorAppCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("'code' is required");

        RuleFor(x => x.OperationType)
            .NotNull().WithMessage("'operation_type' is required")
            .NotEqual(OperationType.None).WithMessage("'operation_type' is invalid");
    }
}