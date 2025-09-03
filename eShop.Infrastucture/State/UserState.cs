using eShop.Domain.Abstraction.State;
using eShop.Domain.DTOs;

namespace eShop.Infrastructure.State;

public class UserState : AsyncState
{
    public Guid UserId { get; set; }
    public UserCredentials? Credentials { get; set; }
    public UserIdentity? Identity { get; set; }
    
    public bool IsAuthenticated => UserId != Guid.Empty;

    public void LogOut()
    {
        UserId = Guid.Empty;
        Credentials = null;
    }

    public void Map(UserStateDto state)
    {
        UserId = state.UserId;
        Credentials = new()
        {
            Email = state.Email,
            RecoveryEmail = state.RecoveryEmail,
            Username = state.Username,
            PhoneNumber = state.PhoneNumber,
        };
        Identity = new()
        {
            Permissions = state.Permissions,
            Roles = state.Roles,
        };
    }
    
    public override async Task Change()
    {
        await StateChanged();
    }
}