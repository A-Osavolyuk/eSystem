using System.IdentityModel.Tokens.Jwt;
using eSecurity.Idp.Security.Authorization.Constants;
using eSecurity.Idp.Security.Authorization.OAuth.Token.TokenExchange.Transformation;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.OAuth.Token.TokenExchange;

public sealed class TransformationTokenExchangeFlow(
    ITokenTransformationHandlerResolver resolver) : ITokenExchangeFlow
{
    private readonly ITokenTransformationHandlerResolver _resolver = resolver;
    private readonly JwtSecurityTokenHandler _handler = new();

    public async ValueTask<Result> ExecuteAsync(TokenExchangeFlowContext context, 
        CancellationToken cancellationToken = default)
    {
        var tokenKind = _handler.CanReadToken(context.SubjectToken) ? TokenKind.Jwt : TokenKind.Opaque;
        var transformationHandler = _resolver.Resolve(tokenKind);
        
        return await transformationHandler.HandleAsync(context, cancellationToken);
    }
}