using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Authorization.Access.Verification;

namespace eSecurity.Common.DTOs;

public class UserVerificationMethod
{
    public bool Preferred { get; set; }
    public VerificationMethod Method { get; set; }
}