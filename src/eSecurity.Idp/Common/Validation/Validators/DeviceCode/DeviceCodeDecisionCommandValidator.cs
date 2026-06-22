using eSecurity.Idp.Features.DeviceCode;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.DeviceCode;

public sealed class DeviceCodeDecisionCommandValidator : AbstractValidator<DeviceCodeDecisionCommand>
{
    public DeviceCodeDecisionCommandValidator()
    {
        RuleFor(x => x.UserCode)
            .NotEmpty().WithMessage("'user_code' is required");

        RuleFor(x => x.Decision)
            .NotNull().WithMessage("'decision' is required")
            .NotEqual(DeviceCodeDecision.None).WithMessage("'decision' is invalid");
    }
}