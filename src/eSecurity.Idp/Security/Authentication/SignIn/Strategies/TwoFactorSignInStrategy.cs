using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Core.Security.Authentication.SignIn;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authentication.SignIn.Strategies;

public sealed class TwoFactorSignInStrategy(
    ITwoFactorContextMapper mapper,
    ITwoFactorStrategyResolver resolver) : SignInStrategy<TwoFactorSignInPayload>
{
    private readonly ITwoFactorContextMapper _mapper = mapper;
    private readonly ITwoFactorStrategyResolver _resolver = resolver;

    public override Type PayloadType => typeof(TwoFactorSignInPayload);

    public override async ValueTask<Result> ExecuteAsync(TwoFactorSignInPayload payload, 
        CancellationToken cancellationToken = default)
    {
        var context = _mapper.Map(payload);
        var strategy = _resolver.Resolve(context);
        return await strategy.ExecuteAsync(context, cancellationToken);
    }
}