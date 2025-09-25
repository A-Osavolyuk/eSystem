namespace eShop.Auth.Api.Entities;

public class LoginMethodEntity : Entity
{
    public Guid Id { get; set; }
    public LoginType Type { get; set; }
}