using eSecurity.Core.Security.Authentication.SignIn;

namespace eSecurity.Idp.Security.Authentication.SignIn;

public sealed class SignInStrategyResolver(
    IServiceProvider serviceProvider, 
    Dictionary<Type, Type> strategies) : ISignInStrategyResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly Dictionary<Type, Type> _strategies = strategies;

    public ISignInStrategy Resolve(SignInPayload payload)
    {
        if (!_strategies.TryGetValue(payload.GetType(), out var strategyType))
            throw new InvalidOperationException("Invalid payload type");

        return (ISignInStrategy)_serviceProvider.GetRequiredService(strategyType);
    }
}