namespace eSecurity.Idp.Security.Authorization.Token.TokenExchange;

public sealed class TokenExchangeFlowResolver(IServiceProvider serviceProvider) : ITokenExchangeFlowResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITokenExchangeFlow Resolve(TokenExchangeFlow flow)
        => _serviceProvider.GetRequiredKeyedService<ITokenExchangeFlow>(flow);
}