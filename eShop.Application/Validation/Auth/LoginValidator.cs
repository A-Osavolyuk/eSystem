using eShop.Domain.Requests.Api.Auth;

namespace eShop.Application.Validation.Auth;

public class LoginValidator : Validator<LoginRequest>
{
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<LoginRequest>
            .CreateWithOptions((LoginRequest)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
    public LoginValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Email is must.")
            .EmailAddress().WithMessage("Invalid format of email address.");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Password is must.");
    }
}