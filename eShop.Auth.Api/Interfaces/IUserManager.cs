namespace eShop.Auth.Api.Interfaces;

public interface IUserManager
{
    public ValueTask<List<UserEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    public ValueTask<Result> ConfirmEmailAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> ConfirmRecoveryEmailAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> ConfirmPhoneNumberAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> ResetPasswordAsync(UserEntity user, string newPassword, CancellationToken cancellationToken = default);
    public ValueTask<Result> ResetEmailAsync(UserEntity user, string newEmail, CancellationToken cancellationToken = default);
    public ValueTask<Result> ResetPhoneNumberAsync(UserEntity user, string newPhoneNumber, CancellationToken cancellationToken = default);
    public ValueTask<Result> RollbackEmailAsync(UserEntity user, string email, CancellationToken cancellationToken = default);
    public ValueTask<Result> RollbackRecoveryEmailAsync(UserEntity user, string recoveryEmail, CancellationToken cancellationToken = default);
    public ValueTask<Result> RollbackPhoneNumberAsync(UserEntity user, string phoneNumber, CancellationToken cancellationToken = default);
    public ValueTask<Result> RollbackPasswordAsync(UserEntity user, string passwordHash, CancellationToken cancellationToken = default);
    public ValueTask<Result> ChangeEmailAsync(UserEntity user, string newEmail, CancellationToken cancellationToken = default);
    public ValueTask<Result> ChangePhoneNumberAsync(UserEntity user, string newPhoneNumber, CancellationToken cancellationToken = default);
    public ValueTask<Result> AddPhoneNumberAsync(UserEntity user, string phoneNumber, CancellationToken cancellationToken = default);
    public ValueTask<Result> AddRecoveryEmailAsync(UserEntity user, string recoveryEmail, CancellationToken cancellationToken = default);
    public ValueTask<Result> CreateAsync(UserEntity user, string password, CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> ChangeNameAsync(UserEntity user, string userName, CancellationToken cancellationToken = default);
    public ValueTask<Result> DeleteAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> ChangePasswordAsync(UserEntity user, string newPassword, CancellationToken cancellationToken = default);
    public ValueTask<bool> CheckPasswordAsync(UserEntity user, string password, CancellationToken cancellationToken = default);
    public ValueTask<bool> CheckNameAsync(string userName, CancellationToken cancellationToken = default);
    public ValueTask<bool> CheckEmailAsync(string email, CancellationToken cancellationToken = default);
    public ValueTask<bool> CheckPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    public string GenerateRandomPassword(int length);
}