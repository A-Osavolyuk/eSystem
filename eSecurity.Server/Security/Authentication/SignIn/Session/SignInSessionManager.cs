using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.SignIn.Session;

public class SignInSessionManager(AuthDbContext context) : ISignInSessionManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<LoginSessionEntity?> FindByIdAsync(Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.LoginSessions.FirstOrDefaultAsync(
            x => x.Id == sessionId, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(LoginSessionEntity session,
        CancellationToken cancellationToken = default)
    {
        await _context.LoginSessions.AddAsync(session, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }
}