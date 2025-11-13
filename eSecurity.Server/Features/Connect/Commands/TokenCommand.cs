using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.Oidc.Token;
using eSecurity.Server.Security.Authentication.Oidc.Token.Strategies;
using eSystem.Core.Security.Authentication.Oidc.Constants;

namespace eSecurity.Server.Features.Connect.Commands;

public record TokenCommand(TokenRequest Request) : IRequest<Result>;

public class TokenCommandHandler(ITokenStrategyResolver tokenStrategyResolver) : IRequestHandler<TokenCommand, Result>
{
    private readonly ITokenStrategyResolver _tokenStrategyResolver = tokenStrategyResolver;

    public async Task<Result> Handle(TokenCommand request, CancellationToken cancellationToken)
    {
        TokenPayload payload = request.Request.GrantType switch
        {
            GrantTypes.AuthorizationCode => new AuthorizationCodeTokenPayload()
            {
                ClientId = request.Request.ClientId,
                GrantType = request.Request.GrantType,
                ClientSecret = request.Request.ClientSecret,
                Code = request.Request.Code,
                CodeVerifier = request.Request.CodeVerifier,
                RedirectUri = request.Request.RedirectUri
            },
            GrantTypes.RefreshToken => new RefreshTokenPayload()
            {
                ClientId = request.Request.ClientId,
                GrantType = request.Request.GrantType,
                ClientSecret = request.Request.ClientSecret,
                RedirectUri = request.Request.RedirectUri,
                RefreshToken = request.Request.RefreshToken
            },
            _ => throw new NotSupportedException("Unsupported grant type")
        };
        
        var strategy = _tokenStrategyResolver.Resolve(request.Request.GrantType);
        return await strategy.ExecuteAsync(payload, cancellationToken);
    }
}