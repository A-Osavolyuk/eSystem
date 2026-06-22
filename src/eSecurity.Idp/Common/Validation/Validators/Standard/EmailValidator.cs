using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Standard;

public sealed class EmailValidator : AbstractValidator<string>
{
    public EmailValidator(string propertyName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        
        RuleFor(x => x)
            .NotEmpty().WithMessage($"'{propertyName}' is required")
            .EmailAddress().WithMessage($"'{propertyName}' is invalid");
    }
}