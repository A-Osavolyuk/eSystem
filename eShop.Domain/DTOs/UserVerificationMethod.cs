namespace eShop.Domain.DTOs;

public class UserVerificationMethod
{
    public bool Preferred { get; set; }
    public VerificationMethod Method { get; set; }
}