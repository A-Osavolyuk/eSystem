using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSystem.Core.Primitives;

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
                Code = ErrorCode.InvalidPayloadType,
                Description = "Invalid payload type"
            });
        }

        var context = _mapper.Map(twoFactorPayload);
        var strategy = _resolver.Resolve(context);
        return await strategy.ExecuteAsync(context, cancellationToken);
    }
}