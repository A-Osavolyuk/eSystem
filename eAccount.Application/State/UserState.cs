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
            PrimaryEmail = state.PrimaryEmail, 
            RecoveryEmail = state.RecoveryEmail,
            PrimaryPhoneNumber = state.PrimaryPhoneNumber,
            RecoveryPhoneNumber = state.RecoveryPhoneNumber,
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
    
    public string? PrimaryEmail { get; set; }
    public string? RecoveryEmail { get; set; }
    
    public string? PrimaryPhoneNumber { get; set; }
    public string? RecoveryPhoneNumber { get; set; }
}

public record UserIdentity(
    IReadOnlyList<RoleDto> Roles,
    IReadOnlyList<PermissionDto> Permissions
)
{
    private bool HasRole(string role) => Roles.Any(r => r.Name == role);
    public bool HasAnyRole(params string[] roles) => roles.Any(HasRole);

    private bool HasPermission(string permission) => Permissions.Any(p => p.Name == permission);
    public bool HasAnyPermission(params string[] permissions) => permissions.Any(HasPermission);
}