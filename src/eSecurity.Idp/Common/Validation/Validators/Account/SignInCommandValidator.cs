using eSecurity.Idp.Features.Account;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Account;

public sealed class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(x => x.Payload)
            .NotNull().WithMessage("'payload' is required")
            .SetValidator(new SignInPayloadValidator());
    }
}