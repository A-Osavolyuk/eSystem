using Microsoft.AspNetCore.DataProtection;

namespace eShop.Auth.Api.Security.Protection;

public class SecretProtector
{
    private readonly IDataProtector protector;
    
    public SecretProtector(IDataProtectionProvider protectionProvider)
    {
        protector = protectionProvider.CreateProtector("TOTP.Secret");
    }
    
    public string Unprotect(string protectedText) => protector.Unprotect(protectedText);
    public string Protect(string unprotectedText) => protector.Protect(unprotectedText);
}