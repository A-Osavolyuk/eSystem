using eSystem.Application.Security.Cryptography.Protection;
using Microsoft.AspNetCore.DataProtection;

namespace eAccount.Blazor.Server.Infrastructure.Security.Cryptography.Protection;

public class SessionProtector(IDataProtectionProvider protectionProvider) : IProtector
{
    private readonly IDataProtector protector = protectionProvider.CreateProtector("SessionProtection");

    public string Protect(string unprotectedText) => protector.Protect(unprotectedText);
    public string Unprotect(string protectedText) => protector.Unprotect(protectedText);
}