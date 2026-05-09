using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Server.Security.Authorization.Par;

public sealed class ParManager(AuthDbContext context) : IParManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<PushedAuthorizationRequestEntity?> FindByIdAsync(Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.PushedAuthorizationRequest
            .Where(x => x.Id == id)
            .Include(x => x.Prompts)
            .Include(x => x.Scopes)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(PushedAuthorizationRequestEntity entity,
        CancellationToken cancellationToken)
    {
        await _context.PushedAuthorizationRequest.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> UpdateAsync(PushedAuthorizationRequestEntity entity,
        CancellationToken cancellationToken)
    {
        _context.PushedAuthorizationRequest.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> RemoveAsync(PushedAuthorizationRequestEntity entity,
        CancellationToken cancellationToken)
    {
        _context.PushedAuthorizationRequest.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }
}