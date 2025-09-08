namespace eShop.Auth.Api.Interfaces;

public interface IUserManager
{
    public ValueTask<List<UserEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByUsernameAsync(string name, CancellationToken cancellationToken = default);

    public ValueTask<UserEntity?> FindByPhoneNumberAsync(string phoneNumber,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> VerifyEmailAsync(UserEntity user, string email, 
        CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyPhoneNumberAsync(UserEntity user, string phoneNumber, 
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ResetPasswordAsync(UserEntity user, string newPassword,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ResetEmailAsync(UserEntity user, string newEmail,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ResetRecoveryEmailAsync(UserEntity user, string newRecoveryEmail,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ResetPhoneNumberAsync(UserEntity user, string newPhoneNumber,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> RemovePhoneNumberAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<Result> RemoveEmailAsync(UserEntity user, string email,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> RemoveRecoveryEmailAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<Result> ChangeEmailAsync(UserEntity user, string currentEmail, string newEmail,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ChangePhoneNumberAsync(UserEntity user, string currentPhoneNumber, string newPhoneNumber,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> AddPhoneNumberAsync(UserEntity user, string phoneNumber,
        bool isPrimary, CancellationToken cancellationToken = default);

    public ValueTask<Result> AddEmailAsync(UserEntity user, string email, bool isPrimary = false,
        bool isRecovery = false, CancellationToken cancellationToken = default);

    public ValueTask<Result> AddPasswordAsync(UserEntity user, string password,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> CreateAsync(UserEntity user, string? password = null,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<Result> ChangeUsernameAsync(UserEntity user, string userName,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> DeleteAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<Result> ChangePasswordAsync(UserEntity user, string newPassword,
        CancellationToken cancellationToken = default);

    public ValueTask<bool> CheckPasswordAsync(UserEntity user, string password,
        CancellationToken cancellationToken = default);

    public ValueTask<bool> IsUsernameTakenAsync(string userName, CancellationToken cancellationToken = default);
    public ValueTask<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default);
    public ValueTask<bool> IsPhoneNumberTakenAsync(string phoneNumber, CancellationToken cancellationToken = default);
}