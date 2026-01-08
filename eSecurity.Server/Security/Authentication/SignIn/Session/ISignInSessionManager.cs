using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.SignIn.Session;

public interface ISignInSessionManager
{
    public ValueTask<SignInSessionEntity?> FindByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    public ValueTask<Result> CreateAsync(SignInSessionEntity session, CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(SignInSessionEntity session, CancellationToken cancellationToken = default);
}