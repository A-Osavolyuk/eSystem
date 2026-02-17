using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authentication.Session;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Server.Security.Authentication.SignIn.Strategies;

public class TwoFactorSignInStrategy(
    ITwoFactorContextMapper mapper,
    ITwoFactorStrategyResolver resolver) : ISignInStrategy
{
    private readonly ITwoFactorContextMapper _mapper = mapper;
    private readonly ITwoFactorStrategyResolver _resolver = resolver;

    public async ValueTask<Result> ExecuteAsync(SignInPayload payload, CancellationToken cancellationToken = default)
    {
        if (payload is not TwoFactorSignInPayload twoFactorPayload)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidPayloadType,
                Description = "Invalid payload type"
            });
        }

        var context = _mapper.Map(twoFactorPayload);
        var strategy = _resolver.Resolve(context);
        return await strategy.ExecuteAsync(context, cancellationToken);
    }
}