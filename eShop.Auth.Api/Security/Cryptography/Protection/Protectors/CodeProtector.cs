using Microsoft.AspNetCore.DataProtection;

namespace eShop.Auth.Api.Security.Cryptography.Protection.Protectors;

public sealed class CodeProtector(IDataProtectionProvider protectionProvider) : Protector
{
    private readonly IDataProtector protector = protectionProvider.CreateProtector(ProtectionPurposes.RecoveryCode);
    public override string Unprotect(string protectedText) => protector.Unprotect(protectedText);
    public override string Protect(string unprotectedText) => protector.Protect(unprotectedText);
}