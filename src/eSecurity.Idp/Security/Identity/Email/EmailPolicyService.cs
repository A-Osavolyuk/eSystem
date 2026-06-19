using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Identity.Email;

public sealed class EmailPolicyService : IEmailPolicyService
{
    public Result CanChangeExistingEmail(Guid userId, UserEmailEntity currentEmail, UserEmailEntity newEmail)
    {
        ArgumentNullException.ThrowIfNull(currentEmail);
        ArgumentNullException.ThrowIfNull(newEmail);
        
        if (currentEmail.UserId != userId || newEmail.UserId != userId)
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

    public Result CanChangeWithNewEmail(Guid userId, UserEmailEntity currentEmail)
    {
        ArgumentNullException.ThrowIfNull(currentEmail);

        if (currentEmail.UserId != userId)
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
}