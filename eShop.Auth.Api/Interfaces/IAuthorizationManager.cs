namespace eShop.Auth.Api.Interfaces;

public interface IAuthorizationManager
{
    public ValueTask<AuthorizationSessionEntity?> FindAsync(UserEntity user, 
        UserDeviceEntity device, CancellationToken cancellationToken);
    
    public ValueTask<Result> CreateAsync(UserEntity user, 
        UserDeviceEntity device, CancellationToken cancellationToken);
    
    public ValueTask<Result> RemoveAsync(AuthorizationSessionEntity session, CancellationToken cancellationToken);
}