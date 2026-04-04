using eSystem.Core.Primitives;

namespace eSecurity.Server.Security.Authorization.Verification;

public interface IVerificationStrategy<in TContext> where TContext : VerificationContext
{
    public ValueTask<Result> ExecuteAsync(TContext context, CancellationToken cancellationToken = default);
}