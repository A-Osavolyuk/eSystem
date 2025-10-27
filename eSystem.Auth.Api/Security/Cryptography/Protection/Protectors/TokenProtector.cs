using eSystem.Core.Security.Cryptography.Protection;
using Microsoft.AspNetCore.DataProtection;

namespace eSystem.Auth.Api.Security.Cryptography.Protection.Protectors;

public class TokenProtector(IDataProtectionProvider protectionProvider) : IProtector
{
    private readonly IDataProtector protector = protectionProvider.CreateProtector(ProtectionPurposes.RefreshToken);
    public string Unprotect(string protectedText) => protector.Unprotect(protectedText);
    public string Protect(string unprotectedText) => protector.Protect(unprotectedText);
}