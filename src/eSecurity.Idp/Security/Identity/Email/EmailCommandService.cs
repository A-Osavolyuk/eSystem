using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Common.Validation;
using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Identity.Email.Extensions;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Identity.Email;

public sealed class EmailCommandService(
    IEmailQueryService query,
    IEmailPolicy policy,
    AuthDbContext context) : IEmailCommandService
{
    private readonly IEmailQueryService _query = query;
    private readonly IEmailPolicy _policy = policy;
    private readonly AuthDbContext _context = context;

    public async ValueTask<Result> AddAsync(Guid userId, string email, EmailType type,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        
        var existingEmail = await _query.FindByEmailAsync(email, cancellationToken);
        if (existingEmail is not null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.EmailTaken,
                Description = "This email address is already taken"
            });
        }
        
        var userEmails = await _query.ListByUserAsync(userId, cancellationToken);
        if (userEmails.All(x => x.NormalizedEmail != Normalizer.Normalize(email)))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "User already has this email"
            });
        }

        var emailsInfo = userEmails.Select(x => x.ToInfo()).ToList();
        var canAddResult = _policy.CanAdd(emailsInfo, type);
        if (!canAddResult.Succeeded)
            return canAddResult;

        var entity = new UserEmailEntity(userId, email, type);
        await _context.UserEmails.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Created);
    }

    public async ValueTask<Result> ChangeAsync(Guid userId, string currentEmail, string newEmail,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currentEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(newEmail);

        var currentEmailEntity = await _query.FindByEmailAsync(currentEmail, cancellationToken);
        var newEmailEntity = await _query.FindByEmailAsync(newEmail, cancellationToken);

        if (currentEmailEntity is null || currentEmailEntity.UserId != userId)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidEmail,
                Description = "Invalid current email"
            });
        }

        if (newEmailEntity is null)
        {
            var currentEmailInfo = currentEmailEntity.ToInfo();
            var changeResult = _policy.CanChangeWithNewEmail(userId, currentEmailInfo);
            if (!changeResult.Succeeded)
                return changeResult;

            currentEmailEntity.Email = newEmail;
            currentEmailEntity.NormalizedEmail = Normalizer.Normalize(newEmail);
        }
        else
        {
            if (newEmailEntity.UserId != userId)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error()
                {
                    Code = ErrorCode.EmailTaken,
                    Description = "This email address is already taken"
                });
            }

            var currentEmailInfo = currentEmailEntity.ToInfo();
            var newEmailInfo = newEmailEntity.ToInfo();
            var canChangeResult = _policy.CanChangeExistingEmail(userId, currentEmailInfo, newEmailInfo);
            if (!canChangeResult.Succeeded) 
                return canChangeResult;

            (currentEmailEntity.Type, newEmailEntity.Type) = (newEmailEntity.Type, currentEmailEntity.Type);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> ResetAsync(Guid userId, string currentEmail, 
        string newEmail, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currentEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(newEmail);

        var currentEmailEntity = await _query.GetByEmailAsync(userId, currentEmail, cancellationToken);
        var newEmailEntity = await _query.GetByEmailAsync(userId, newEmail, cancellationToken);
        
        if (currentEmailEntity is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidEmail,
                Description = "Current email is invalid"
            });
        }

        if (newEmailEntity is not null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.EmailTaken,
                Description = "New email is already taken"
            });
        }

        var currentEmailInfo = currentEmailEntity.ToInfo();
        var canResetResult = _policy.CanReset(userId, currentEmailInfo);
        if (!canResetResult.Succeeded) return canResetResult;

        currentEmailEntity.Email = newEmail;
        currentEmailEntity.NormalizedEmail = Normalizer.Normalize(newEmail);

        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> VerifyAsync(Guid userId, string email, 
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var emailEntity = await _query.FindByEmailAsync(email, cancellationToken);
        if (emailEntity is null || emailEntity.UserId != userId)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidEmail,
                Description = "Invalid email"
            });
        }

        if (emailEntity.IsVerified)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.UnverifiedEmail,
                Description = "This email address is already verified"
            });
        }

        emailEntity.IsVerified = true;
        emailEntity.VerifiedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> RemoveAsync(Guid userId, string email, 
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var emailEntity = await _query.FindByEmailAsync(email, cancellationToken);
        if (emailEntity is null || emailEntity.UserId != userId)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidEmail,
                Description = "Invalid email"
            });
        }

        _context.UserEmails.Remove(emailEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }
}