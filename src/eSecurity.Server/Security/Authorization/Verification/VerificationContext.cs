using eSecurity.Core.Security.Authorization.Verification;

namespace eSecurity.Server.Security.Authorization.Verification;

public abstract class VerificationContext
{
    public ActionType Action { get; set; }
    public PurposeType Purpose { get; set; }
}