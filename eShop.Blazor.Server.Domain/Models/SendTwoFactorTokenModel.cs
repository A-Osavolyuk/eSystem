using eShop.Domain.Security.Authentication.TwoFactor;

namespace eShop.Blazor.Server.Domain.Models;

public class SendTwoFactorTokenModel
{
    public TwoFactorMethod Type { get; set; }
}