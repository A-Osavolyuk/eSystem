using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Security.Identity.Options;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Identity.Email;

public sealed class EmailPolicy(IOptions<AccountOptions> options) : IEmailPolicy
{
    private readonly AccountOptions _options = options.Value;

    public Result CanChangeExistingEmail(Guid userId, EmailInfo currentEmail, EmailInfo newEmail)
    {
        ArgumentNullException.ThrowIfNull(currentEmail);
        ArgumentNullException.ThrowIfNull(newEmail);

        if (currentEmail.OwnerId != userId || newEmail.OwnerId != userId)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidEmail,
                Description = "User must own both email addresses"
            });
        }

        if (!currentEmail.IsVerified)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.UnverifiedEmail,
                Description = "First email address is not verified"
            });
        }

        if (!newEmail.IsVerified)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.UnverifiedEmail,
                Description = "Second email address is not verified"
            });
        }

        if (currentEmail.Type == EmailType.Secondary)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidEmail,
                Description = "It is now allowed to change secondary emails"
            });
        }

        if (newEmail.Type != EmailType.Secondary)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidEmail,
                Description = "You can change your current email only with your other secondary email"
            });
        }

        //TODO: Implement check if user have linked external accounts, when trying to change primary email

        return Results.Success(SuccessCodes.Ok);
    }

    public Result CanChangeWithNewEmail(Guid userId, EmailInfo currentEmail)
    {
        ArgumentNullException.ThrowIfNull(currentEmail);

        if (currentEmail.OwnerId != userId)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidEmail,
                Description = "User must own both email addresses"
            });
        }

        if (!currentEmail.IsVerified)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.UnverifiedEmail,
                Description = "First email address is not verified"
            });
        }

        if (currentEmail.Type == EmailType.Secondary)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidEmail,
                Description = "It is now allowed to change secondary emails"
            });
        }

        //TODO: Implement check if user have linked external accounts, when trying to change primary email

        return Results.Success(SuccessCodes.Ok);
    }

    public Result CanReset(Guid userId, EmailInfo currentEmail)
    {
        ArgumentNullException.ThrowIfNull(currentEmail);

        if (!currentEmail.IsVerified)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.UnverifiedEmail,
                Description = "This email address is not verified yet"
            });
        }

        if (currentEmail.Type == EmailType.Secondary)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidEmail,
                Description = "You cannot reset secondary email"
            });
        }

        //TODO: Implement check if user have linked external accounts, when trying to change primary email

        return Results.Success(SuccessCodes.Ok);
    }

    public Result CanAdd(List<EmailInfo> userEmails, EmailType type)
    {
        ArgumentNullException.ThrowIfNull(userEmails);

        if (type == EmailType.Secondary && Count(type) >= _options.SecondaryEmailMaxCount)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "You cannot add secondary email"
            });
        }
        
        if (type == EmailType.Primary && Count(type) >= _options.PrimaryEmailMaxCount)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "You cannot add primary email"
            });
        }

        if (type == EmailType.Recovery && Count(type) >= _options.RecoveryEmailMaxCount)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "You cannot add recovery email"
            });
        }

        return Results.Success(SuccessCodes.Ok);
        
        int Count(EmailType emailType)
        {
            return userEmails.Count(x => x.Type == emailType);
        }
    }
}