using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authorization.LinkedAccount;

public sealed class LinkedAccountCommandService(
    AuthDbContext context, 
    ILinkedAccountQueryService query) : ILinkedAccountCommandService
{
    private readonly AuthDbContext _context = context;
    private readonly ILinkedAccountQueryService _query = query;

    public async ValueTask<Result> CreateAsync(Guid userId, LinkedAccountType type,
        CancellationToken cancellationToken = default)
    {
        var existingLinkedAccounts = await _query.ListByUserAsync(userId, cancellationToken);
        if (existingLinkedAccounts.Any(x => x.Type == type))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "User already has this linked account"
            });
        }

        var entity = new UserLinkedAccountEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Type = type
        };

        await _context.UserLinkedAccounts.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Created);
    }

    public async ValueTask<Result> RemoveAsync(Guid linkedAccountId, CancellationToken cancellationToken = default)
    {
        var entity = await _query.GetByIdAsync(linkedAccountId, cancellationToken);
        if (entity is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid linked account"
            });
        }

        _context.UserLinkedAccounts.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }
}