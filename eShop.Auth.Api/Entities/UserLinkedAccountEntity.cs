namespace eShop.Auth.Api.Entities;

public class UserLinkedAccountEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public LinkedAccountType Type { get; set; }
    public bool Allowed { get; set; }

    public UserEntity User { get; set; } = null!;
}