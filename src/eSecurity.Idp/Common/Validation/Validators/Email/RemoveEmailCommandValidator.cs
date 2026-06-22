using eSecurity.Idp.Common.Validation.Validators.Standard;
using eSecurity.Idp.Features.Email;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Email;

public sealed class RemoveEmailCommandValidator : AbstractValidator<RemoveEmailCommand>
{
    public RemoveEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .SetValidator(new EmailValidator("email"));
    }
}