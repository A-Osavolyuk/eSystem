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
    
    public ValueTask<Result> SetEmailAsync(UserEntity user, string email, 
        EmailType type, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> SetPhoneNumberAsync(UserEntity user, string phoneNumber, 
        PhoneNumberType type, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> SetUsernameAsync(UserEntity user, string username, 
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ManageEmailAsync(UserEntity user, EmailType type, 
        string email, CancellationToken cancellationToken = default);

    public ValueTask<Result> VerifyEmailAsync(UserEntity user, string email, 
        CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyPhoneNumberAsync(UserEntity user, string phoneNumber, 
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ResetEmailAsync(UserEntity user, string currentEmail, string newEmail,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ResetPhoneNumberAsync(UserEntity user, string currentEmail, string newPhoneNumber,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> RemovePhoneNumberAsync(UserEntity user, string phoneNumber, 
        CancellationToken cancellationToken = default);

    public ValueTask<Result> RemoveEmailAsync(UserEntity user, string email,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ChangeEmailAsync(UserEntity user, string currentEmail, string newEmail,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ChangePhoneNumberAsync(UserEntity user, string currentPhoneNumber, string newPhoneNumber,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> AddPhoneNumberAsync(UserEntity user, string phoneNumber,
        PhoneNumberType type, CancellationToken cancellationToken = default);

    public ValueTask<Result> AddEmailAsync(UserEntity user, string email, 
        EmailType type, CancellationToken cancellationToken = default);

    public ValueTask<Result> CreateAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<Result> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<Result> ChangeUsernameAsync(UserEntity user, string userName,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> DeleteAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<bool> IsUsernameTakenAsync(string userName, CancellationToken cancellationToken = default);
    public ValueTask<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default);
    public ValueTask<bool> IsPhoneNumberTakenAsync(string phoneNumber, CancellationToken cancellationToken = default);
}