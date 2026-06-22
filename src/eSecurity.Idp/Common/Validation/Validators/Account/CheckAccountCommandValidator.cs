using eSecurity.Idp.Features.Account;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Account;

public sealed class CheckAccountCommandValidator : AbstractValidator<CheckAccountCommand>
{
    public CheckAccountCommandValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("'login' is required");
    }
}