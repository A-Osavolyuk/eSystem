using eSecurity.Core.Security.Authorization.OAuth;
using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class UserLinkedAccountEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public LinkedAccountType Type { get; set; }

    public UserEntity User { get; set; } = null!;
}