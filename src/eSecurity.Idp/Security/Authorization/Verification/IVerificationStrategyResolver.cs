using eSecurity.Core.Security.Authorization.Verification;

namespace eSecurity.Idp.Security.Authorization.Verification;

public interface IVerificationStrategyResolver
{
    public IVerificationStrategy Resolve(VerificationMethod verificationMethod);
}