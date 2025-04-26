namespace eShop.Auth.Api.Extensions;

public static class UserManagerExtensions
{
    public static async Task<UserEntity?> FindByIdAsync(this UserManager<UserEntity> userManager, Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        return user;
    }

    public static async Task<UserEntity?> FindByPhoneNumberAsync(this UserManager<UserEntity> userManager, string phoneNumber)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        return user;
    }

    public static async Task<LockoutStatus> GetLockoutStatusAsync(this UserManager<UserEntity> userManager, UserEntity userEntity)
    {
        var lockoutEnabled = await userManager.GetLockoutEnabledAsync(userEntity);
        var lockoutEnd = await userManager.GetLockoutEndDateAsync(userEntity);

        return new LockoutStatus() { LockoutEnabled = lockoutEnabled, LockoutEnd = lockoutEnd };
    }

    public static async Task<IdentityResult> UnlockUserAsync(this UserManager<UserEntity> userManager, UserEntity userEntity)
    {
        var lockoutEnabled = await userManager.SetLockoutEnabledAsync(userEntity, false);
        var lockoutEnd = await userManager.SetLockoutEndDateAsync(userEntity, new DateTime(1980, 1, 1));

        return IdentityResult.Success;
    }

    public static async Task<IdentityResult> RemoveFromRolesAsync(this UserManager<UserEntity> userManager, UserEntity userEntity)
    {
        var roles = await userManager.GetRolesAsync(userEntity);

        if (roles.Any())
        {
            var result = await userManager.RemoveFromRolesAsync(userEntity, roles);

            if (!result.Succeeded)
            {
                return result;
            }
        }

        return IdentityResult.Success;
    }

    public static async Task<IdentityResult> ResetPasswordAsync(this UserManager<UserEntity> userManager, UserEntity userEntity,
        string password)
    {
        var token = await userManager.GeneratePasswordResetTokenAsync(userEntity);
        var result = await userManager.ResetPasswordAsync(userEntity, token, password);
        return result;
    }

    public static async Task<IdentityResult> ConfirmEmailAsync(this UserManager<UserEntity> userManager, UserEntity userEntity)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(userEntity);
        var result = await userManager.ConfirmEmailAsync(userEntity, token);
        return result;
    }

    public static async Task<IdentityResult> ConfirmPhoneNumberAsync(this UserManager<UserEntity> userManager,
        UserEntity userEntity)
    {
        userEntity.PhoneNumberConfirmed = true;
        await userManager.UpdateAsync(userEntity);
        return IdentityResult.Success;
    }

    public static async Task<IdentityResult> ChangeEmailAsync(this UserManager<UserEntity> userManager, UserEntity userEntity,
        string newEmail)
    {
        var token = await userManager.GenerateChangeEmailTokenAsync(userEntity, newEmail);
        var result = await userManager.ChangeEmailAsync(userEntity, newEmail, token);
        return result;
    }

    public static async Task<IdentityResult> ChangePhoneNumberAsync(this UserManager<UserEntity> userManager, UserEntity userEntity,
        string newPhoneNumber)
    {
        var token = await userManager.GenerateChangePhoneNumberTokenAsync(userEntity, newPhoneNumber);
        var result = await userManager.ChangePhoneNumberAsync(userEntity, newPhoneNumber, token);
        return result;
    }
}