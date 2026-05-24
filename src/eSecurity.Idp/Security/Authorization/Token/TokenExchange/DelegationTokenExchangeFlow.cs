using System.IdentityModel.Tokens.Jwt;
using eSecurity.Idp.Security.Authorization.Constants;
using eSecurity.Idp.Security.Authorization.Token.TokenExchange.Delegation;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Token.TokenExchange;

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