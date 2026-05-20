using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Core.Security.Authentication.SignIn;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authentication.SignIn.Strategies;

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
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
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