using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Verification;

public interface IVerificationPolicy
{
    Result CanConsume(VerificationRequestInfo request);

    Result CanApprove(VerificationRequestInfo request);

    Result CanCancel(VerificationRequestInfo request);
}