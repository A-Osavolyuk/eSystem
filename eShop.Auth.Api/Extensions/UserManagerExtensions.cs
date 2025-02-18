namespace eShop.Auth.Api.Extensions;

public static class UserManagerExtensions
{
    public static async Task<AppUser?> FindByIdAsync(this UserManager<AppUser> userManager, Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        return user;
    }

    public static async Task<AppUser?> FindByPhoneNumberAsync(this UserManager<AppUser> userManager, string phoneNumber)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        return user;
    }

    public static async Task<LockoutStatus> GetLockoutStatusAsync(this UserManager<AppUser> userManager, AppUser user)
    {
        var lockoutEnabled = await userManager.GetLockoutEnabledAsync(user);
        var lockoutEnd = await userManager.GetLockoutEndDateAsync(user);

        return new LockoutStatus() { LockoutEnabled = lockoutEnabled, LockoutEnd = lockoutEnd };
    }

    public static async Task<IdentityResult> UnlockUserAsync(this UserManager<AppUser> userManager, AppUser user)
    {
        var lockoutEnabled = await userManager.SetLockoutEnabledAsync(user, false);
        var lockoutEnd = await userManager.SetLockoutEndDateAsync(user, new DateTime(1980, 1, 1));

        return IdentityResult.Success;
    }

    public static async Task<IdentityResult> RemoveFromRolesAsync(this UserManager<AppUser> userManager, AppUser user)
    {
        var roles = await userManager.GetRolesAsync(user);

        if (roles.Any())
        {
            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                return result;
            }
        }

        return IdentityResult.Success;
    }

    public static async Task<IdentityResult> ResetPasswordAsync(this UserManager<AppUser> userManager, AppUser user,
        string password)
    {
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, password);
        return result;
    }

    public static async Task<IdentityResult> ConfirmEmailAsync(this UserManager<AppUser> userManager, AppUser user)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var result = await userManager.ConfirmEmailAsync(user, token);
        return result;
    }

    public static async Task<IdentityResult> ConfirmPhoneNumberAsync(this UserManager<AppUser> userManager,
        AppUser user)
    {
        user.PhoneNumberConfirmed = true;
        await userManager.UpdateAsync(user);
        return IdentityResult.Success;
    }

    public static async Task<IdentityResult> ChangeEmailAsync(this UserManager<AppUser> userManager, AppUser user,
        string newEmail)
    {
        var token = await userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        var result = await userManager.ChangeEmailAsync(user, newEmail, token);
        return result;
    }

    public static async Task<IdentityResult> ChangePhoneNumberAsync(this UserManager<AppUser> userManager, AppUser user,
        string newPhoneNumber)
    {
        var token = await userManager.GenerateChangePhoneNumberTokenAsync(user, newPhoneNumber);
        var result = await userManager.ChangePhoneNumberAsync(user, newPhoneNumber, token);
        return result;
    }
}