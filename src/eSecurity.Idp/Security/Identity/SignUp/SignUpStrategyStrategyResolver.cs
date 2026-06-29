namespace eSecurity.Idp.Security.Identity.SignUp;

public class SignUpStrategyStrategyResolver(
    IServiceProvider serviceProvider, 
    Dictionary<Type, Type> strategies) : ISignUpStrategyResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly Dictionary<Type, Type> _strategies = strategies;

    public ISignUpStrategy Resolve(SignUpPayload payload)
    {
        if (!_strategies.TryGetValue(payload.GetType(), out var strategyType))
            throw new InvalidOperationException("Invalid payload type");

        return (ISignUpStrategy)_serviceProvider.GetRequiredService(strategyType);
    }
}