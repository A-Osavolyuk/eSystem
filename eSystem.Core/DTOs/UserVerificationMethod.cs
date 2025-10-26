using eSystem.Core.Security.Verification;

namespace eSystem.Core.DTOs;

public class UserVerificationMethod
{
    public bool Preferred { get; set; }
    public VerificationMethod Method { get; set; }
}