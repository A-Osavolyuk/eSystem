using eShop.Domain.Enums;

namespace eShop.BlazorWebUI.Models;

public class ConfirmForgotPasswordModel
{
    public string Code { get; set; } = string.Empty;
    public CodeType Type { get; set; }
}