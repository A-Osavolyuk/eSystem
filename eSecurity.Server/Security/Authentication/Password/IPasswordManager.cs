using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Password;

public interface IPasswordManager
{
    public ValueTask<Result> AddAsync(UserEntity user, 
        string password, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> ChangeAsync(UserEntity user, 
        string newPassword, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> ResetAsync(UserEntity user, 
        string newPassword, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> RemoveAsync(UserEntity user, CancellationToken cancellationToken = default);
    
    public bool Check(UserEntity user, string password);
}