using eShop.Domain.Abstraction.State;

namespace eShop.Infrastructure.State;

public class UserStateContainer : AsyncStateContainer
{
    public Guid UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? RecoveryEmail { get; set; }
    public bool IsAuthenticated => UserId != Guid.Empty;
    
    public override async Task Change()
    {
        await StateChanged();
    }
}