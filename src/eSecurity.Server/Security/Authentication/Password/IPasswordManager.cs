using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Password;

public interface IPasswordManager
{
    public ValueTask<PasswordEntity?> GetAsync(UserEntity user, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> AddAsync(UserEntity user, 
        string password, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> ChangeAsync(UserEntity user, 
        string newPassword, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> ResetAsync(UserEntity user, 
        string newPassword, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> RemoveAsync(UserEntity user, CancellationToken cancellationToken = default);
    
    public ValueTask<bool> HasAsync(UserEntity user, CancellationToken cancellationToken = default);
    
    public ValueTask<bool> CheckAsync(UserEntity user, string password, CancellationToken cancellationToken = default);
}