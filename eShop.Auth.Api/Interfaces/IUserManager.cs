namespace eShop.Auth.Api.Interfaces;

public interface IUserManager
{
    public ValueTask<List<UserEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);

    public ValueTask<UserEntity?> FindByPhoneNumberAsync(string phoneNumber,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ConfirmEmailAsync(UserEntity user, string code,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ConfirmPhoneNumberAsync(UserEntity user, string code,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ResetPasswordAsync(UserEntity user, string newPassword, CancellationToken cancellationToken = default);

    public ValueTask<Result> ChangeEmailAsync(UserEntity user, string currentEmailCode, string newEmailCode,
        string newEmail, CancellationToken cancellationToken = default);

    public ValueTask<Result> ChangePhoneNumberAsync(UserEntity user, string currentPhoneNumberCode,
        string newPhoneNumberCode, string newPhoneNumber, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> AddPhoneNumberAsync(UserEntity user, string phoneNumber, CancellationToken cancellationToken = default);

    public ValueTask<Result> CreateAsync(UserEntity user, string password,
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<Result> ChangeUsernameAsync(UserEntity user, string userName,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> DeleteAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<bool> CheckPasswordAsync(UserEntity user, string password,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ChangePasswordAsync(UserEntity user, string newPassword, CancellationToken cancellationToken = default);
    
    public string GenerateRandomPassword(int length);
}