using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Identity.User;

public interface IUserManager
{
    public ValueTask<UserEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByUsernameAsync(string name, CancellationToken cancellationToken = default);
    
    public ValueTask<UserEntity?> FindByPhoneNumberAsync(string phoneNumber,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> CreateAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<Result> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<Result> DeleteAsync(UserEntity user, CancellationToken cancellationToken = default);
}