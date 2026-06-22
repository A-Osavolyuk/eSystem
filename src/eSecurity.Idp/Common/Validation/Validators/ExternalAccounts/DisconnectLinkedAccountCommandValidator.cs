using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Idp.Features.LinkedAccounts;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.ExternalAccounts;

public sealed class DisconnectLinkedAccountCommandValidator : AbstractValidator<DisconnectLinkedAccountCommand>
{
    public DisconnectLinkedAccountCommandValidator()
    {
        RuleFor(x => x.Type)
            .NotNull().WithMessage("'type' is required")
            .NotEqual(LinkedAccountType.None).WithMessage("'type' is invalid");
    }
}