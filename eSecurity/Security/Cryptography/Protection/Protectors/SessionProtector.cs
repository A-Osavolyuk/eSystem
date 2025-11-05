using eSystem.Core.Security.Cryptography.Protection;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Security.Cryptography.Protection.Protectors;

public class SessionProtector(IDataProtectionProvider protectionProvider) : IProtector
{
    private readonly IDataProtector protector = protectionProvider.CreateProtector(ProtectionPurposes.Session);

    public string Protect(string unprotectedText) => protector.Protect(unprotectedText);
    public string Unprotect(string protectedText) => protector.Unprotect(protectedText);
}