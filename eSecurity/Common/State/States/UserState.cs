using eSystem.Core.DTOs;

namespace eSecurity.Common.State.States;

public class UserState : State
{
    public Guid UserId { get; set; }
    public UserCredentials? Credentials { get; set; }
    public UserIdentity? Identity { get; set; }

    public bool IsAuthenticated => UserId != Guid.Empty;

    public void Clear()
    {
        UserId = Guid.Empty;
        Credentials = null;
        Identity = null;
    }

    public void Map(UserStateDto state)
    {
        UserId = state.UserId;
        Identity = new UserIdentity(state.Roles, state.Permissions);
        Credentials = new UserCredentials
        {
            Username = state.Username, 
            Email = state.Email, 
            PhoneNumber = state.PhoneNumber,
        };
    }

    public override async Task Change()
    {
        await StateChanged();
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