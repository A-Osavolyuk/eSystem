using eSecurity.Idp.Features.DeviceCode;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.DeviceCode;

public sealed class CheckDeviceCodeCommandValidator : AbstractValidator<CheckDeviceCodeCommand>
{
    public CheckDeviceCodeCommandValidator()
    {
        RuleFor(x => x.UserCode)
            .NotEmpty().WithMessage("'user_code' is required");
    }
}