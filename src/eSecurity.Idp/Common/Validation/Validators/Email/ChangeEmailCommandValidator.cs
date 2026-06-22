using eSecurity.Idp.Common.Validation.Validators.Standard;
using eSecurity.Idp.Features.Email.Change;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Email;

public sealed class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
{
    public ChangeEmailCommandValidator()
    {
        RuleFor(x => x.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("'code' is required");

        RuleFor(x => x.CurrentEmail)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new EmailValidator("current_email"));

        RuleFor(x => x.NewEmail)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new EmailValidator("new_email"));
    }
}