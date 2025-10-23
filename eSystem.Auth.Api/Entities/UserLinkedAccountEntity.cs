using eSystem.Domain.Security.Authorization.OAuth;

namespace eSystem.Auth.Api.Entities;

public class UserLinkedAccountEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public LinkedAccountType Type { get; set; }

    public UserEntity User { get; set; } = null!;
}