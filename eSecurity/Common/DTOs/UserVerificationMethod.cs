using eSystem.Core.Security.Authorization.Access;

namespace eSecurity.Common.DTOs;

public class UserVerificationMethod
{
    public bool Preferred { get; set; }
    public VerificationMethod Method { get; set; }
}