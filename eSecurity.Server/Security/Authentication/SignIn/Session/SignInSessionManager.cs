using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.SignIn.Session;

public class SignInSessionManager(AuthDbContext context) : ISignInSessionManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<SignInSessionEntity?> FindByIdAsync(Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.SignInSessions.FirstOrDefaultAsync(
            x => x.Id == sessionId, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(SignInSessionEntity session,
        CancellationToken cancellationToken = default)
    {
        await _context.SignInSessions.AddAsync(session, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }

    public async ValueTask<Result> UpdateAsync(SignInSessionEntity session, 
        CancellationToken cancellationToken = default)
    {
        _context.SignInSessions.Update(session);
        await _context.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }
}