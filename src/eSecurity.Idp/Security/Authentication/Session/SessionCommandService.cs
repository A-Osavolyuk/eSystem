using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authentication.Session;

public sealed class SessionCommandService(AuthDbContext context) : ISessionCommandService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<Result> CreateAsync(SessionEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.Sessions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Created);
    }

    public async ValueTask<Result> RemoveAsync(SessionEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Sessions.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }
}