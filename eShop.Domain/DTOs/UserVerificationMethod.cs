using eShop.Domain.Security.Verification;

namespace eShop.Domain.DTOs;

public class UserVerificationMethod
{
    public bool Preferred { get; set; }
    public VerificationMethod Method { get; set; }
}