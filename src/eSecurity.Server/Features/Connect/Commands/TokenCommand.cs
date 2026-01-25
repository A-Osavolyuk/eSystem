using eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Strategies;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Authentication.OpenIdConnect.Token;

namespace eSecurity.Server.Features.Connect.Commands;

public record TokenCommand(TokenRequest Request) : IRequest<Result>;

public class TokenCommandHandler(
    ITokenStrategyResolver tokenStrategyResolver,
    IOptions<OpenIdConfiguration> options) : IRequestHandler<TokenCommand, Result>
{
    private readonly ITokenStrategyResolver _tokenStrategyResolver = tokenStrategyResolver;
    private readonly OpenIdConfiguration _configuration = options.Value;

    public async Task<Result> Handle(TokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Request.GrantType))
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "grant_type is required"
            });

        if (string.IsNullOrEmpty(request.Request.ClientId))
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "client_id is required"
            });

        if (!_configuration.GrantTypesSupported.Contains(request.Request.GrantType))
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = $"'{request.Request.GrantType}' grant type is not supported"
            });

        TokenPayload? payload = request.Request.GrantType switch
        {
            GrantTypes.AuthorizationCode => new AuthorizationCodeTokenPayload()
            {
                ClientId = request.Request.ClientId,
                GrantType = request.Request.GrantType,
                CodeVerifier = request.Request.CodeVerifier,
                RedirectUri = request.Request.RedirectUri,
                Code = request.Request.Code
            },
            GrantTypes.RefreshToken => new RefreshTokenPayload()
            {
                ClientId = request.Request.ClientId,
                GrantType = request.Request.GrantType,
                RedirectUri = request.Request.RedirectUri,
                RefreshToken = request.Request.RefreshToken,
                Scope = request.Request.Scope
            },
            _ => null
        };

        if (payload is null)
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.UnsupportedGrantType,
                Description = $"'{request.Request.GrantType}' grant type is not supported"
            });

        var strategy = _tokenStrategyResolver.Resolve(request.Request.GrantType);
        return await strategy.ExecuteAsync(payload, cancellationToken);
    }
}