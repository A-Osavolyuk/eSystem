using eSystem.Core.Security.Authentication.TwoFactor;

namespace eAccount.Blazor.Server.Domain.Models;

public class SendTwoFactorTokenModel
{
    public TwoFactorMethod Type { get; set; }
}