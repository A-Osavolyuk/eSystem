using Microsoft.AspNetCore.DataProtection;

namespace eShop.Auth.Api.Security.Protection;

public class SecretProtector(IDataProtectionProvider protectionProvider)
{
    private readonly IDataProtector protector = protectionProvider.CreateProtector("TOTP.Secret");
    
    public string Unprotect(string protectedText) => protector.Unprotect(protectedText);
    public string Protect(string unprotectedText) => protector.Protect(unprotectedText);
}