using Microsoft.AspNetCore.DataProtection;

namespace eShop.Auth.Api.Security.Protection;

public class CodeProtector(IDataProtectionProvider protectionProvider)
{
    private readonly IDataProtector protector = protectionProvider.CreateProtector("2FA.RecoveryCodes");
    
    public string Unprotect(string protectedText) => protector.Unprotect(protectedText);
    public string Protect(string unprotectedText) => protector.Protect(unprotectedText);
}