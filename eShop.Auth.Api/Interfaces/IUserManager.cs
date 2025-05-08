namespace eShop.Auth.Api.Interfaces;

public interface IUserManager
{
    public ValueTask<List<UserEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    public ValueTask<Result> ConfirmEmailAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> ConfirmPhoneNumberAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> ResetPasswordAsync(UserEntity user, string newPassword, CancellationToken cancellationToken = default);
    public ValueTask<Result> ChangeEmailAsync(UserEntity user, string newEmail, CancellationToken cancellationToken = default);
    public ValueTask<Result> ChangePhoneNumberAsync(UserEntity user, string newPhoneNumber, CancellationToken cancellationToken = default);
    public ValueTask<Result> CreateAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> CreateAsync(UserEntity user, string password, CancellationToken cancellationToken = default);
    public ValueTask<Result> SetUserNameAsync(UserEntity user, string userName, CancellationToken cancellationToken = default);
    public ValueTask<Result> AddPasswordAsync(UserEntity user, string password, CancellationToken cancellationToken = default);
    public ValueTask<Result> DeleteAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<bool> CheckPasswordAsync(UserEntity user, string password, CancellationToken cancellationToken = default);
    public ValueTask<Result> ChangePasswordAsync(UserEntity user, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    
    //Role methods
    public ValueTask<bool> IsInRoleAsync(UserEntity user, string roleName, CancellationToken cancellationToken = default);
    public ValueTask<Result> AddToRoleAsync(UserEntity user, string roleName, CancellationToken cancellationToken = default);
    public ValueTask<Result> AddToRoleAsync(UserEntity user, RoleEntity role, CancellationToken cancellationToken = default);
    public ValueTask<Result> RemoveFromRoleAsync(UserEntity user, string roleName, CancellationToken cancellationToken = default);
    public ValueTask<Result> RemoveFromRoleAsync(UserEntity user, RoleEntity role, CancellationToken cancellationToken = default);
    public ValueTask<Result> RemoveFromRolesAsync(UserEntity user, CancellationToken cancellationToken = default);
    
    //TwoFactor methods
    public ValueTask<Result> SetTwoFactorEnabledAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<string> GenerateTwoFactorTokenAsync(UserEntity user, string provider, CancellationToken cancellationToken = default);
    public ValueTask<bool> VerifyTwoFactorTokenAsync(UserEntity user, string provider, string token, CancellationToken cancellationToken = default);
}