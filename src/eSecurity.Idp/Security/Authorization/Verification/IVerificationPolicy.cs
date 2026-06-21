using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Verification;

public interface IVerificationPolicy
{
    Result CanConsume(VerificationRequestEntity request);

    Result CanApprove(VerificationRequestEntity request);

    Result CanCancel(VerificationRequestEntity request);
}