namespace eShop.Auth.Api.Interfaces;

public interface ILoginSessionManager
{
    public ValueTask CreateAsync(UserDeviceEntity device, LoginType type, 
        string provider, CancellationToken cancellationToken = default);
    
    public ValueTask CreateAsync(UserDeviceEntity device, LoginType type, 
        CancellationToken cancellationToken = default);
}