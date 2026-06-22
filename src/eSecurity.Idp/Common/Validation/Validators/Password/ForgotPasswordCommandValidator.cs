using eSecurity.Idp.Common.Validation.Validators.Standard;
using eSecurity.Idp.Features.Password;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Password;

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .SetValidator(new EmailValidator("email"));
    }
}