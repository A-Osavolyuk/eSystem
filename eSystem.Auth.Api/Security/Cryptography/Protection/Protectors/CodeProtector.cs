using eSystem.Application.Security.Cryptography.Protection;
using Microsoft.AspNetCore.DataProtection;

namespace eSystem.Auth.Api.Security.Cryptography.Protection.Protectors;

public sealed class CodeProtector(IDataProtectionProvider protectionProvider) : IProtector
{
    private readonly IDataProtector protector = protectionProvider.CreateProtector(ProtectionPurposes.RecoveryCode);
    public string Unprotect(string protectedText) => protector.Unprotect(protectedText);
    public string Protect(string unprotectedText) => protector.Protect(unprotectedText);
}