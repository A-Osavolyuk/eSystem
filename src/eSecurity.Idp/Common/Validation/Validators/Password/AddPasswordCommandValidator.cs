using eSecurity.Idp.Features.Password;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Password;

public sealed class AddPasswordCommandValidator : AbstractValidator<AddPasswordCommand>
{
    public AddPasswordCommandValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("'password' is required");
    }
}