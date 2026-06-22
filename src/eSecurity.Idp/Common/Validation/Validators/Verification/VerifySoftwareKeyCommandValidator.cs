using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Features.Verification.SoftwareKey;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Verification;

public sealed class VerifySoftwareKeyCommandValidator : AbstractValidator<VerifySoftwareKeyCommand>
{
    public VerifySoftwareKeyCommandValidator()
    {
        RuleFor(x => x.Credential)
            .NotNull().WithMessage("'credentials' is required");

        RuleFor(x => x.OperationType)
            .NotNull().WithMessage("'operation_type' is required")
            .NotEqual(OperationType.None).WithMessage("'operation_type' is invalid");
    }
}