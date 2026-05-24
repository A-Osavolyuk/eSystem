using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Verification;

public interface IVerificationStrategy
{
    public ValueTask<Result> ExecuteAsync(VerificationContext context, CancellationToken cancellationToken = default);
}