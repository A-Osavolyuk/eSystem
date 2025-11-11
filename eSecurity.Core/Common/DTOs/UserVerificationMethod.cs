using eSecurity.Core.Security.Authorization.Access;

namespace eSecurity.Core.Common.DTOs;

public class UserVerificationMethod
{
    public bool Preferred { get; set; }
    public VerificationMethod Method { get; set; }
}