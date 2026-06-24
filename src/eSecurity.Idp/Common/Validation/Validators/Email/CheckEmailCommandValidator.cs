using eSecurity.Idp.Features.Email;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Email;

public sealed class CheckEmailCommandValidator : AbstractValidator<CheckEmailCommand>
{
    public CheckEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("'email' is required")
            .EmailAddress().WithMessage("'email' is invalid");
    }
}