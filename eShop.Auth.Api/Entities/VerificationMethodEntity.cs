namespace eShop.Auth.Api.Entities;

public class VerificationMethodEntity : Entity
{
    public Guid Id { get; set; }
    public VerificationMethod Method { get; set; }
}