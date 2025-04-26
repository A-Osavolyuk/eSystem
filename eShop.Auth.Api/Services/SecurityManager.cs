namespace eShop.Auth.Api.Services;

internal sealed class SecurityManager(
    AuthDbContext context,
    UserManager<UserEntity> userManager) : ISecurityManager
{
    private readonly AuthDbContext context = context;
    private readonly UserManager<UserEntity> userManager = userManager;

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
    public async ValueTask<string> GenerateVerificationCodeAsync(string destination,
        Verification codeType)
    {
        var code = GenerateCode();
        await SaveCodeAsync(code, destination, codeType);
        return code;
    }
    public async ValueTask<CodeSet> GenerateVerificationCodeSetAsync(DestinationSet destinationSet,
        Verification codeType)
    {
        var codeSet = new CodeSet()
        {
            Current = await GenerateVerificationCodeAsync(destinationSet.Current, codeType),
            Next = await GenerateVerificationCodeAsync(destinationSet.Next, codeType)
        };

        return codeSet;
    }
    public async ValueTask<IdentityResult> VerifyEmailAsync(UserEntity userEntity, string code)
    {
        var validationResult = await ValidateAndRemoveAsync(code, userEntity.Email!, Verification.VerifyEmail);

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }

        var result = await userManager.ConfirmEmailAsync(userEntity);

        if (!result.Succeeded)
        {
            return result;
        }

        return IdentityResult.Success;
    }
    public async ValueTask<IdentityResult> VerifyPhoneNumberAsync(UserEntity userEntity, string code)
    {
        var validationResult = await ValidateAndRemoveAsync(code, userEntity.Email!, Verification.VerifyPhoneNumber);

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }

        var result = await userManager.ConfirmPhoneNumberAsync(userEntity);

        if (!result.Succeeded)
        {
            return result;
        }

        return IdentityResult.Success;
    }
    public async ValueTask<IdentityResult> ResetPasswordAsync(UserEntity userEntity, string code, string password)
    {
        var validationResult = await ValidateAndRemoveAsync(code, userEntity.Email!, Verification.VerifyEmail);

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }

        var result = await userManager.ResetPasswordAsync(userEntity, password);

        if (!result.Succeeded)
        {
            return result;
        }

        return IdentityResult.Success;
    }
    public async ValueTask<IdentityResult> ChangeEmailAsync(UserEntity userEntity, string newEmail, CodeSet codeSet)
    {
        var destinationSet = new DestinationSet() { Current = userEntity.Email!, Next = newEmail };
        var validationResult = await ValidateAndRemoveAsync(codeSet, destinationSet, Verification.ChangeEmail);

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }

        var result = await userManager.ChangeEmailAsync(userEntity, newEmail);
        return result;
    }
    public async ValueTask<IdentityResult> ChangePhoneNumberAsync(UserEntity userEntity, string newPhoneNumber, CodeSet codeSet)
    {
        var destinationSet = new DestinationSet() { Current = userEntity.PhoneNumber!, Next = newPhoneNumber };
        var validationResult =
            await ValidateAndRemoveAsync(codeSet, destinationSet, Verification.ChangePhoneNumber);

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }

        var result = await userManager.ChangePhoneNumberAsync(userEntity, newPhoneNumber);
        return result;
    }
    public async ValueTask<VerificationCodeEntity?> FindCodeAsync(string destination, Verification codeType)
    {
        var entity = await context.Codes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Destination == destination && c.CodeType == codeType);

        return entity;
    }
    public async ValueTask<IdentityResult> VerifyCodeAsync(string code, string destination, Verification type)
    {
        var entity = await context.Codes
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.Destination == destination
                     && x.Code == code
                     && x.CodeType == type
                     && x.ExpireDate < DateTime.UtcNow);

        if (entity is null)
        {
            return IdentityResult.Failed(new IdentityError { Code = "404", Description = "Cannot find code" });
        }

        return IdentityResult.Success;
    }
    public async ValueTask<SecurityTokenEntity?> FindTokenAsync(UserEntity userEntity)
    {
        var entity = await context.SecurityTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userEntity.Id);

        return entity;
    }
    public async ValueTask<IdentityResult> RemoveTokenAsync(UserEntity userEntity)
    {
        var token = await context.SecurityTokens.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userEntity.Id);

        if (token is null)
        {
            return IdentityResult.Failed(new IdentityError() { Code = "404", Description = "Cannot find token" });
        }

        context.SecurityTokens.Remove(token);
        await context.SaveChangesAsync();

        return IdentityResult.Success;
    }
    public async ValueTask SaveTokenAsync(UserEntity userEntity, string token, DateTime validTo)
    {
        await context.SecurityTokens.AddAsync(new()
        {
            UserId = userEntity.Id, 
            Token = token,
            ExpireDate = validTo,
        });
        await context.SaveChangesAsync();
    }

    #region Private methods

    private string GenerateCode()
    {
        var code = new Random().Next(100000, 999999).ToString();
        return code;
    }

    private async Task SaveCodeAsync(string code, string sentTo, Verification verification)
    {
        await context.Codes.AddAsync(new VerificationCodeEntity()
        {
            Id = Guid.CreateVersion7(),
            Destination = sentTo,
            Code = code,
            CodeType = verification,
            CreateDate = DateTime.UtcNow,
            ExpireDate = DateTime.UtcNow.AddMinutes(10)
        });

        await context.SaveChangesAsync();
    }

    private async Task<VerificationCodeEntity?> FindCodeAsync(string code, string sentTo, Verification verification)
    {
        var entity = await context.Codes
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.Code == code && c.Destination == sentTo && c.CodeType == verification);

        return entity;
    }

    private async Task RemoveCodeAsync(VerificationCodeEntity entity)
    {
        context.Codes.Remove(entity);
        await context.SaveChangesAsync();
    }

    private async Task<IdentityResult> ValidateAndRemoveAsync(string code, string sentTo,
        Verification verification)
    {
        var entity = await FindCodeAsync(code, sentTo, verification);

        if (entity is null)
        {
            return IdentityResult.Failed(new IdentityError()
            {
                Code = "404",
                Description = "Cannot find code"
            });
        }

        if (entity.ExpireDate < DateTime.UtcNow)
        {
            return IdentityResult.Failed(new IdentityError()
            {
                Code = "400",
                Description = $"Code is already expired"
            });
        }

        await RemoveCodeAsync(entity);

        return IdentityResult.Success;
    }

    private async Task<IdentityResult> ValidateAndRemoveAsync(CodeSet codeSet, DestinationSet destinationSet,
        Verification verification)
    {
        var currentResult = await ValidateAndRemoveAsync(codeSet.Current, destinationSet.Current, verification);
        var nextResult = await ValidateAndRemoveAsync(codeSet.Next, destinationSet.Next, verification);

        if (!currentResult.Succeeded)
        {
            return currentResult;
        }

        if (!nextResult.Succeeded)
        {
            return nextResult;
        }

        return IdentityResult.Success;
    }

    #endregion
}