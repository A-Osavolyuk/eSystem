namespace eShop.Auth.Api.Interfaces;

public interface ILoginSessionManager
{
    public ValueTask CreateAsync(UserDeviceEntity device, LoginStatus status, LoginType type, 
        string provider, CancellationToken cancellationToken = default);
    
    public ValueTask CreateAsync(UserDeviceEntity device, LoginStatus status, LoginType type, 
         CancellationToken cancellationToken = default);
}