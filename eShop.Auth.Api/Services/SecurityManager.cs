namespace eShop.Auth.Api.Services;

internal sealed class SecurityManager(
    AuthDbContext context,
    UserManager<UserEntity> userManager,
    ICodeManager codeManager) : ISecurityManager
{
    private readonly AuthDbContext context = context;
    private readonly UserManager<UserEntity> userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;

    public string GenerateRandomPassword(int length)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!-_";
        var sb = new StringBuilder();
        var random = new Random();

        for (var i = 0; i < length; i++)
        {
            var randomIndex = random.Next(validChars.Length);
            sb.Append(validChars[randomIndex]);
        }

        return sb.ToString();
    }

    public async ValueTask<Result> VerifyEmailAsync(UserEntity userEntity, string code)
    {
        var validationResult = await ValidateAndRemoveAsync(userEntity, code);

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }

        var result = await userManager.ConfirmEmailAsync(userEntity);

        if (!result.Succeeded)
        {
            return Results.InternalServerError(result.Errors.First().Description);
        }

        return Result.Success();
    }

    public async ValueTask<Result> VerifyPhoneNumberAsync(UserEntity userEntity, string code)
    {
        var validationResult = await ValidateAndRemoveAsync(userEntity, code);

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }

        var result = await userManager.ConfirmPhoneNumberAsync(userEntity);

        if (!result.Succeeded)
        {
            return Results.InternalServerError(result.Errors.First().Description);
        }

        return Result.Success();
    }

    public async ValueTask<Result> ResetPasswordAsync(UserEntity userEntity, string code, string password)
    {
        var validationResult = await ValidateAndRemoveAsync(userEntity, code);

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }

        var result = await userManager.ResetPasswordAsync(userEntity, password);

        if (!result.Succeeded)
        {
            return Results.InternalServerError(result.Errors.First().Description);
        }

        return Result.Success();
    }

    public async ValueTask<Result> ChangeEmailAsync(UserEntity userEntity, string newEmail, CodeSet codeSet)
    {
        var currentCodeValidationResult = await ValidateAndRemoveAsync(userEntity, codeSet.Current);
        var nextCodeValidationResult = await ValidateAndRemoveAsync(userEntity, codeSet.Next);

        if (!currentCodeValidationResult.Succeeded)
        {
            return currentCodeValidationResult;
        }
        
        if (!nextCodeValidationResult.Succeeded)
        {
            return nextCodeValidationResult;
        }

        var result = await userManager.ChangeEmailAsync(userEntity, newEmail);
        
        if (!result.Succeeded)
        {
            return Results.InternalServerError(result.Errors.First().Description);
        }

        return Result.Success();
    }

    public async ValueTask<Result> ChangePhoneNumberAsync(UserEntity userEntity, string newPhoneNumber,
        CodeSet codeSet)
    {
        var currentCodeValidationResult = await ValidateAndRemoveAsync(userEntity, codeSet.Current);
        var nextCodeValidationResult = await ValidateAndRemoveAsync(userEntity, codeSet.Next);

        if (!currentCodeValidationResult.Succeeded)
        {
            return currentCodeValidationResult;
        }
        
        if (!nextCodeValidationResult.Succeeded)
        {
            return nextCodeValidationResult;
        }

        var result = await userManager.ChangePhoneNumberAsync(userEntity, newPhoneNumber);
        
        if (!result.Succeeded)
        {
            return Results.InternalServerError(result.Errors.First().Description);
        }

        return Result.Success();
    }

    private async Task<Result> ValidateAndRemoveAsync(UserEntity user, string code)
    {
        var entity = await codeManager.FindAsync(user, code);

        if (entity is null)
        {
            return Results.NotFound("Code not found");
        }

        var validationResult = codeManager.Validate(entity);

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }

        await codeManager.DeleteAsync(entity);

        return Result.Success();
    }
}