using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authorization.Token;

public sealed class TokenCommandService(AuthDbContext context, ITokenQueryService query) : ITokenCommandService
{
    private readonly AuthDbContext _context = context;
    private readonly ITokenQueryService _query = query;

    public async ValueTask<Result> CreateAsync(OpaqueTokenEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.OpaqueTokens.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> RevokeAsync(Guid tokenId, CancellationToken cancellationToken = default)
    {
        var tokenEntity = await _query.GetByIdAsync(tokenId, cancellationToken);
        if (tokenEntity is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidToken,
                Description = "Invalid token"
            });
        }

        tokenEntity.Revoked = true;
        tokenEntity.RevokedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }
}