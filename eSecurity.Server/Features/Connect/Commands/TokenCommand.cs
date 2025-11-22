using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authentication.Oidc;
using eSecurity.Server.Security.Authentication.Oidc.Token;
using eSecurity.Server.Security.Authentication.Oidc.Token.Strategies;
using eSystem.Core.Security.Authentication.Oidc.Token;

namespace eSecurity.Server.Features.Connect.Commands;

public record TokenCommand(TokenRequest Request) : IRequest<Result>;

public class TokenCommandHandler(
    ITokenStrategyResolver tokenStrategyResolver,
    IOptions<OpenIdOptions> options) : IRequestHandler<TokenCommand, Result>
{
    private readonly ITokenStrategyResolver _tokenStrategyResolver = tokenStrategyResolver;
    private readonly OpenIdOptions _options = options.Value;

    public async Task<Result> Handle(TokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Request.GrantType))
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "grant_type is required"
            });

        if (string.IsNullOrEmpty(request.Request.ClientId))
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "client_id is required"
            });

        if (!_options.GrantTypesSupported.Contains(request.Request.GrantType))
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidGrant,
                Description = $"'{request.Request.GrantType}' grant type is not supported"
            });

        TokenPayload? payload = request.Request.GrantType switch
        {
            GrantTypes.AuthorizationCode => new AuthorizationCodeTokenPayload()
            {
                ClientId = request.Request.ClientId,
                GrantType = request.Request.GrantType,
                ClientSecret = request.Request.ClientSecret,
                CodeVerifier = request.Request.CodeVerifier,
                RedirectUri = request.Request.RedirectUri,
                Code = request.Request.Code
            },
            GrantTypes.RefreshToken => new RefreshTokenPayload()
            {
                ClientId = request.Request.ClientId,
                GrantType = request.Request.GrantType,
                ClientSecret = request.Request.ClientSecret,
                RedirectUri = request.Request.RedirectUri,
                RefreshToken = request.Request.RefreshToken,
                Scope = request.Request.Scope
            },
            _ => null
        };

        if (payload is null)
            return Results.BadRequest(new Error
            {
                Code = Errors.OAuth.UnsupportedGrantType,
                Description = $"'{request.Request.GrantType}' grant type is not supported"
            });

        var strategy = _tokenStrategyResolver.Resolve(request.Request.GrantType);
        return await strategy.ExecuteAsync(payload, cancellationToken);
    }
}