namespace eShop.Auth.Api.Entities;

public class TwoFactorMethodEntity : Entity
{
    public Guid Id { get; init; }
    public MethodType Type { get; set; }
}
