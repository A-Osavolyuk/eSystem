using eSecurity.Idp.Features.Account;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Account;
 
public class CompleteSignUpCommandValidator : AbstractValidator<CompleteSignUpCommand>
{
    public CompleteSignUpCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("'code' is required");
    }
}