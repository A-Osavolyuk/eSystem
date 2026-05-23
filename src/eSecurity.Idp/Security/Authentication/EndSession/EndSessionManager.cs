using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authentication.EndSession;

public sealed class EndSessionManager(AuthDbContext context) : IEndSessionManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<EndSessionRequestEntity?> FindByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.EndSessionRequests
            .Where(x => x.Id == id)
            .Include(x => x.UiLocales)
            .Include(x => x.Client)
            .Include(x => x.Session)
            .Include(x => x.User)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<EndSessionRequestEntity?> FindByIdAsync(string id,
        CancellationToken cancellationToken = default)
    {
        return await FindByIdAsync(Guid.Parse(id), cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(EndSessionRequestEntity entity,
        CancellationToken cancellationToken = default)
    {
        await _context.EndSessionRequests.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> UpdateAsync(EndSessionRequestEntity entity,
        CancellationToken cancellationToken = default)
    {
        _context.EndSessionRequests.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> RemoveAsync(EndSessionRequestEntity entity,
        CancellationToken cancellationToken = default)
    {
        _context.EndSessionRequests.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }
}