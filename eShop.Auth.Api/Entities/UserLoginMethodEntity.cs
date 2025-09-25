namespace eShop.Auth.Api.Entities;

public class UserLoginMethodEntity : Entity
{
    public Guid UserId { get; set; }
    public Guid MethodId { get; set; }

    public UserEntity User { get; set; } = null!;
    public LoginMethodEntity Method { get; set; } = null!;
}