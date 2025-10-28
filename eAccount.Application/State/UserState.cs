using eAccount.Domain.Abstraction.State;

namespace eAccount.Application.State;

public class UserState : AsyncState
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
            Email = state.PrimaryEmail, 
            PhoneNumber = state.PrimaryPhoneNumber,
        };
    }

    public override async Task Change()
    {
        await StateChanged();
    }
}

public class UserCredentials
{
    public string? Username { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
}

public record UserIdentity(
    IReadOnlyList<RoleDto> Roles,
    IReadOnlyList<PermissionDto> Permissions
);