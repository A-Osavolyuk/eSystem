using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authorization.OAuth.Token.RefreshToken;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.RefreshToken;

public sealed class OAuthRefreshTokenFlow(
    IClientManager clientManager,
    ITokenManager tokenManager,
    IUserManager userManager,
    ITokenFactoryProvider tokenFactoryProvider,
    IOptions<TokenConfigurations> options) : IRefreshTokenFlow
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IUserManager _userManager = userManager;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;

    public async ValueTask<Result> ExecuteAsync(OpaqueTokenEntity token, RefreshTokenFlowContext flowContext,
        CancellationToken cancellationToken = default)
    {
        var client = await _clientManager.FindByIdAsync(flowContext.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.Unauthorized(new Error
            {
                Code = ErrorTypes.OAuth.InvalidClient,
                Description = "Invalid client."
            });
        }

        if (!client.HasGrantType(flowContext.GrantType))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.UnsupportedGrantType,
                Description = $"'{flowContext.GrantType}' is not supported by client."
            });
        }

        if (client.Id != token.ClientId)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid refresh token"
            });
        }

        if (!client.AllowOfflineAccess)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Refresh token grant is not allowed for this client."
            });
        }

        var user = await _userManager.FindByIdAsync(Guid.Parse(token.Subject), cancellationToken);
        if (user is null)
        {
            return Results.NotFound(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid refresh token."
            });
        }

        var response = new RefreshTokenResponse
        {
            ExpiresIn = (int)_tokenConfigurations.DefaultAccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenTypes.Bearer,
        };

        var accessTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.AccessToken);
        var accessTokenResult = await accessTokenFactory.CreateAsync(client, 
            user, cancellationToken: cancellationToken);
        
        if (!accessTokenResult.Succeeded)
        {
            var error = accessTokenResult.GetError();
            return Results.InternalServerError(error);
        }

        if (!accessTokenResult.TryGetValue(out var accessToken))
        {
            return Results.InternalServerError(new Error()
            {
                Code = ErrorTypes.OAuth.ServerError,
                Description = "Server error"
            });
        }
            
        response.AccessToken = accessToken;

        if (client.RefreshTokenRotationEnabled)
        {
            var refreshTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.RefreshToken);
            var refreshTokenResult = await refreshTokenFactory.CreateAsync(client, 
                user, cancellationToken: cancellationToken);
            
            if (!refreshTokenResult.Succeeded)
            {
                var error = refreshTokenResult.GetError();
                return Results.InternalServerError(error);
            }

            if (!refreshTokenResult.TryGetValue(out var refreshToken))
            {
                return Results.InternalServerError(new Error()
                {
                    Code = ErrorTypes.OAuth.ServerError,
                    Description = "Server error"
                });
            }
            
            response.RefreshToken = refreshToken;

            var revokeResult = await _tokenManager.RevokeAsync(token, cancellationToken);
            return revokeResult.Succeeded ? Results.Ok(response) : revokeResult;
        }

        response.RefreshToken = flowContext.RefreshToken;

        return Results.Ok(response);
    }
}