using eSystem.Core.Security.Cryptography.Protection;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Security.Cryptography.Protection.Protectors;

public sealed class SecretProtector(IDataProtectionProvider protectionProvider) : IProtector
{
    private readonly IDataProtector protector = protectionProvider.CreateProtector(ProtectionPurposes.Secret);
    public string Unprotect(string protectedText) => protector.Unprotect(protectedText);
    public string Protect(string unprotectedText) => protector.Protect(unprotectedText);
}