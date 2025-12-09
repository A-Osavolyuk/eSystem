using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Identity.User;

public interface IUserManager
{
    public ValueTask<UserEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByUsernameAsync(string name, CancellationToken cancellationToken = default);
    
    public ValueTask<UserEntity?> FindByPhoneNumberAsync(string phoneNumber,
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> SetPhoneNumberAsync(UserEntity user, string phoneNumber, 
        PhoneNumberType type, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> SetUsernameAsync(UserEntity user, string username, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> VerifyPhoneNumberAsync(UserEntity user, string phoneNumber, 
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ResetPhoneNumberAsync(UserEntity user, string currentEmail, string newPhoneNumber,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> RemovePhoneNumberAsync(UserEntity user, string phoneNumber, 
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ChangePhoneNumberAsync(UserEntity user, string currentPhoneNumber, string newPhoneNumber,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> AddPhoneNumberAsync(UserEntity user, string phoneNumber,
        PhoneNumberType type, CancellationToken cancellationToken = default);

    public ValueTask<Result> CreateAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<Result> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<Result> ChangeUsernameAsync(UserEntity user, string userName,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> DeleteAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<bool> IsUsernameTakenAsync(string userName, CancellationToken cancellationToken = default);
    public ValueTask<bool> IsPhoneNumberTakenAsync(string phoneNumber, CancellationToken cancellationToken = default);
}