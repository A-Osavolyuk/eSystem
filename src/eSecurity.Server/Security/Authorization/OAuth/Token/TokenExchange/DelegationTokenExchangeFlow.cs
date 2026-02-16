using System.IdentityModel.Tokens.Jwt;
using eSecurity.Server.Security.Authorization.Constants;
using eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Delegation;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange;

public sealed class DelegationTokenExchangeFlow(
    ITokenDelegationHandlerResolver resolver) : ITokenExchangeFlow
{
    private readonly ITokenDelegationHandlerResolver _resolver = resolver;
    private readonly JwtSecurityTokenHandler _handler = new JwtSecurityTokenHandler();

    public async ValueTask<Result> ExecuteAsync(TokenExchangeFlowContext context, 
        CancellationToken cancellationToken = default)
    {
        var tokenKind = _handler.CanReadToken(context.SubjectToken) ? TokenKind.Jwt : TokenKind.Opaque;
        var delegationHandler = _resolver.Resolve(tokenKind);
        
        return await delegationHandler.HandleAsync(context, cancellationToken);
    }
}