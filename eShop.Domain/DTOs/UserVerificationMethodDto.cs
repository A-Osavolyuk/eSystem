namespace eShop.Domain.DTOs;

public class UserVerificationMethodDto
{
    public bool Preferred { get; set; }
    public VerificationMethod Method { get; set; }
}