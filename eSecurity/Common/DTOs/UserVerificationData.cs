using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Authorization.Access.Verification;

namespace eSecurity.Common.DTOs;

public class UserVerificationData
{
    public VerificationMethod PreferredMethod { get; set; }
    public bool EmailEnabled { get; set; }
    public bool AuthenticatorEnabled { get; set; }
    public bool PasskeyEnabled { get; set; }
    public List<UserVerificationMethod> Methods { get; set; } = [];
}