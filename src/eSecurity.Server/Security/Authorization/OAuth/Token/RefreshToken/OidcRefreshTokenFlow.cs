using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token.RefreshToken;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.RefreshToken;

public sealed class OidcRefreshTokenFlow(
    ITokenBuilderProvider tokenBuilderProvider,
    IClientManager clientManager,
    ITokenManager tokenManager,
    IUserManager userManager,
    IHasherProvider hasherProvider,
    ISessionManager sessionManager,
    IClaimFactoryProvider claimFactoryProvider,
    ITokenFactoryProvider tokenFactoryProvider,
    IOptions<TokenConfigurations> options) : IRefreshTokenFlow
{
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;
    private readonly IClientManager _clientManager = clientManager;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IUserManager _userManager = userManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Sha512);

    public async ValueTask<Result> ExecuteAsync(OpaqueTokenEntity token, RefreshTokenFlowContext flowContext,
        CancellationToken cancellationToken = default)
    {
        if (!token.SessionId.HasValue)
        {
            return Results.NotFound(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid refresh token."
            });
        }

        var session = await _sessionManager.FindByIdAsync(token.SessionId.Value, cancellationToken);
        if (session is null)
        {
            return Results.NotFound(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid refresh token."
            });
        }

        if (token.Revoked)
        {
            var sessionResult = await _sessionManager.RemoveAsync(session, cancellationToken);
            if (!sessionResult.Succeeded) return sessionResult;

            return Results.NotFound(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid refresh token."
            });
        }

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

        if (!client.HasScope(ScopeTypes.OfflineAccess))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidScope,
                Description = "offline_access scope was not originally granted."
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
        var accessTokenResult = await accessTokenFactory.CreateAsync(client, user, 
            session, cancellationToken: cancellationToken);
        
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
            var refreshTokenResult = await refreshTokenFactory.CreateAsync(client, user, 
                session, cancellationToken: cancellationToken);
            
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
        }
        else
        {
            response.RefreshToken = flowContext.RefreshToken;
        }

        var idTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.IdToken);
        var idTokenResult = await idTokenFactory.CreateAsync(client, user, 
            session, cancellationToken: cancellationToken);
        
        if (!idTokenResult.Succeeded)
        {
            var error = idTokenResult.GetError();
            return Results.InternalServerError(error);
        }

        if (!idTokenResult.TryGetValue(out var idToken))
        {
            return Results.InternalServerError(new Error()
            {
                Code = ErrorTypes.OAuth.ServerError,
                Description = "Server error"
            });
        }
            
        response.IdToken = idToken;

        return Results.Ok(response);
    }
}