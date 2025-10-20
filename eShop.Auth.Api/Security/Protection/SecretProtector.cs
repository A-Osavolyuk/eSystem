using Microsoft.AspNetCore.DataProtection;

namespace eShop.Auth.Api.Security.Protection;

public sealed class SecretProtector(IDataProtectionProvider protectionProvider) : Protector
{
    private readonly IDataProtector protector = protectionProvider.CreateProtector(ProtectionPurposes.Secret);
    public override string Unprotect(string protectedText) => protector.Unprotect(protectedText);
    public override string Protect(string unprotectedText) => protector.Protect(unprotectedText);
}