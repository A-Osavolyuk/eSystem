using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class ConfirmChangeEmailValidator : Validator<ConfirmChangeEmailModel>
{
    public ConfirmChangeEmailValidator()
    {
        RuleFor(x => x.CurrentEmailCode)
            .Length(6).WithMessage("Code must be 6 digits long");
        
        RuleFor(x => x.NewEmailCode)
            .Length(6).WithMessage("Code must be 6 digits long");
    }
}