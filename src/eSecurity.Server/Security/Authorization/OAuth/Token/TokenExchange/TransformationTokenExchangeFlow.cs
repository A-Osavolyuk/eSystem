using System.IdentityModel.Tokens.Jwt;
using eSecurity.Server.Security.Authorization.Constants;
using eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Transformation;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange;

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