using eSystem.Auth.Api.Entities;
using eSystem.Domain.Security.Authentication;

namespace eSystem.Auth.Api.Interfaces;

public interface ILoginManager
{
    public ValueTask CreateAsync(UserDeviceEntity device, LoginType type, 
        string provider, CancellationToken cancellationToken = default);
    
    public ValueTask CreateAsync(UserDeviceEntity device, LoginType type, 
        CancellationToken cancellationToken = default);
}