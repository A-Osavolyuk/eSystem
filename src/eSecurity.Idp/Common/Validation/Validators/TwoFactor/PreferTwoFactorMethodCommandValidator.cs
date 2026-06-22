using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Idp.Features.TwoFactor;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.TwoFactor;

public sealed class PreferTwoFactorMethodCommandValidator : AbstractValidator<PreferTwoFactorMethodCommand>
{
    public PreferTwoFactorMethodCommandValidator()
    {
        RuleFor(x => x.PreferredMethod)
            .NotNull().WithMessage("'preferred_method' is required")
            .NotEqual(TwoFactorMethod.None).WithMessage("'preferred_method' is invalid");
    }
}