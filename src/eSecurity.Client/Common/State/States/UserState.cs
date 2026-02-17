using eSecurity.Core.Common.DTOs;

namespace eSecurity.Client.Common.State.States;

public class UserState : State
{
    public Guid UserId { get; set; }
    public string? Subject { get; set; }
    public UserCredentials? Credentials { get; set; }
    public UserIdentity? Identity { get; set; }

    public bool IsAuthenticated => UserId != Guid.Empty;

    public void Clear()
    {
        UserId = Guid.Empty;
        Credentials = null;
        Identity = null;
    }
}

public class UserCredentials
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}

public record UserIdentity(
    IReadOnlyList<RoleDto> Roles,
    IReadOnlyList<PermissionDto> Permissions
);