namespace eSecurity.Idp.Security.Identity.SignUp;

public class SignUpStrategyStrategyResolver : ISignUpStrategyResolver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, Type> _strategies;

    public SignUpStrategyStrategyResolver(IServiceProvider serviceProvider, IEnumerable<ISignUpStrategy> strategies)
    {
        _serviceProvider = serviceProvider;
        _strategies = strategies.ToDictionary(x => x.PayloadType, x => x.GetType());
    }

    public ISignUpStrategy Resolve(SignUpPayload payload)
    {
        if (!_strategies.TryGetValue(payload.GetType(), out var strategyType))
            throw new InvalidOperationException("Invalid payload type");

        return (ISignUpStrategy)_serviceProvider.GetRequiredService(strategyType);
    }
}