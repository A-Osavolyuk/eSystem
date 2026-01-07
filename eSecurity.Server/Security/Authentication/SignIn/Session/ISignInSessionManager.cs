using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.SignIn.Session;

public interface ISignInSessionManager
{
    public ValueTask<LoginSessionEntity?> FindByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    public ValueTask<Result> CreateAsync(LoginSessionEntity session, CancellationToken cancellationToken = default);
}