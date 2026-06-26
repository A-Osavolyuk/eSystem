using eSecurity.Core.Security.Authentication.SignIn;

namespace eSecurity.Idp.Security.Authentication.SignIn;

public sealed class SignInStrategyResolver : ISignInStrategyResolver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, Type> _strategies;

    public SignInStrategyResolver(IServiceProvider serviceProvider, IEnumerable<ISignInStrategy> strategies)
    {
        _serviceProvider = serviceProvider;
        _strategies = strategies.ToDictionary(x => x.PayloadType, x => x.GetType());
    }

    public ISignInStrategy Resolve(SignInPayload payload)
    {
        if (!_strategies.TryGetValue(payload.GetType(), out var strategyType))
            throw new InvalidOperationException("Invalid payload type");

        return (ISignInStrategy)_serviceProvider.GetRequiredService(strategyType);
    }
}