using eSystem.Domain.Security.Verification;

namespace eSystem.Domain.DTOs;

public class UserVerificationMethod
{
    public bool Preferred { get; set; }
    public VerificationMethod Method { get; set; }
}