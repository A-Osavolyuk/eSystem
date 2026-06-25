using eSecurity.Core.Security.Authentication.TwoFactor;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authentication.TwoFactor;

public sealed class TwoFactorPolicy : ITwoFactorPolicy
{
    public Result CanAddMethod(IReadOnlyCollection<TwoFactorMethodInfo> methods, TwoFactorMethod method)
    {
        ArgumentNullException.ThrowIfNull(methods);
        
        if (method == TwoFactorMethod.None)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid 2FA method"
            });
        }
        
        if (methods.Count > 0 && methods.Any(x => x.Method == method))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "User is already subscribed to this 2FA method"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
    
    public Result CanSetPreferredMethod(IReadOnlyCollection<TwoFactorMethodInfo> methods,
        TwoFactorMethodInfo method)
    {
        ArgumentNullException.ThrowIfNull(methods);
        ArgumentNullException.ThrowIfNull(method);
        
        if (method.Method == TwoFactorMethod.None)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid 2FA method"
            });
        }

        if (method.Method == TwoFactorMethod.RecoveryCode)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Recovery code cannot be preferred 2FA method"
            });
        }
        
        var currentPreferredMethod = methods.FirstOrDefault(x => x.IsPreferred);
        if (currentPreferredMethod is null)
            return Results.Success(SuccessCodes.Ok);
        
        if (currentPreferredMethod.Priority > method.Priority)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Cannot change current preferred 2FA method"
            });
        }

        return Results.Success(SuccessCodes.Ok);
    }
}