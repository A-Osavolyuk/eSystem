using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Pkce;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.Claims.Factories;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token.AuthorizationCode;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.AuthorizationCode;

public class OAuthAuthorizationCodeFlow(
    IUserManager userManager,
    IPkceHandler pkceHandler,
    IClientManager clientManager,
    IClaimFactoryProvider claimFactoryProvider,
    ITokenFactoryProvider tokenFactoryProvider,
    IAuthorizationCodeManager authorizationCodeManager,
    IOptions<TokenOptions> options) : IAuthorizationCodeFlow
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPkceHandler _pkceHandler = pkceHandler;
    private readonly IClientManager _clientManager = clientManager;
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly IAuthorizationCodeManager _authorizationCodeManager = authorizationCodeManager;
    private readonly TokenOptions _configurations = options.Value;

    public async ValueTask<Result> ExecuteAsync(AuthorizationCodeEntity code, 
        AuthorizationCodeFlowContext context, CancellationToken cancellationToken = default)
    {
        var client = await _clientManager.FindByIdAsync(context.ClientId, cancellationToken);
        if (client is null)
            return Results.Unauthorized(new Error
            {
                Code = ErrorTypes.OAuth.InvalidClient,
                Description = "Client was not found."
            });

        if (client.Id != code.ClientId || !client.HasUri(context.RedirectUri, UriType.Redirect))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        if (!client.HasGrantType(context.GrantType))
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.UnsupportedGrantType,
                Description = $"'{context.GrantType}' grant is not supported by client."
            });

        var user = await _userManager.FindByIdAsync(code.UserId, cancellationToken);
        if (user is null)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        if (client is { ClientType: ClientType.Public, RequirePkce: true })
        {
            if (string.IsNullOrWhiteSpace(code.CodeChallenge)
                || string.IsNullOrWhiteSpace(code.CodeChallengeMethod)
                || string.IsNullOrWhiteSpace(context.CodeVerifier))
            {
                return Results.BadRequest(new Error
                {
                    Code = ErrorTypes.OAuth.InvalidGrant,
                    Description = "Invalid authorization code."
                });
            }

            var isValidPkce = _pkceHandler.Verify(
                code.CodeChallenge,
                code.CodeChallengeMethod,
                context.CodeVerifier
            );

            if (!isValidPkce)
            {
                return Results.BadRequest(new Error
                {
                    Code = ErrorTypes.OAuth.InvalidGrant,
                    Description = "Invalid authorization code."
                });
            }
        }

        var codeResult = await _authorizationCodeManager.UseAsync(code, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        var response = new AuthorizationCodeResponse()
        {
            ExpiresIn = (int)_configurations.DefaultAccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenTypes.Bearer,
        };

        if (client.AccessTokenType == AccessTokenType.Jwt)
        {
            var claimsFactory = _claimFactoryProvider.GetClaimFactory<AccessTokenClaimsContext, UserEntity>();
            var claims = await claimsFactory.GetClaimsAsync(user, new AccessTokenClaimsContext
            {
                Aud = client.Audiences.Select(x => x.Audience),
                Scopes = client.AllowedScopes.Select(x => x.Scope.Value),
            }, cancellationToken);

            var tokenContext = new JwtTokenContext { Claims = claims, Type = JwtTokenTypes.AccessToken };
            var tokenFactory = _tokenFactoryProvider.GetFactory<JwtTokenContext, string>();

            response.AccessToken = await tokenFactory.CreateTokenAsync(tokenContext, cancellationToken);
        }
        else
        {
            var tokenContext = new OpaqueTokenContext
            {
                TokenLength = _configurations.OpaqueTokenLength,
                TokenType = OpaqueTokenType.AccessToken,
                ClientId = client.Id,
                Audiences = client.Audiences.Select(x => x.Audience).ToList(),
                Scopes = client.AllowedScopes.Select(x => x.Scope.Value).ToList(),
                ExpiredAt = DateTimeOffset.UtcNow.Add(_configurations.DefaultAccessTokenLifetime),
                Subject = user.Id.ToString(),
            };
            
            var tokenFactory = _tokenFactoryProvider.GetFactory<OpaqueTokenContext, string>();
            response.AccessToken = await tokenFactory.CreateTokenAsync(tokenContext, cancellationToken);
        }

        if (client.AllowOfflineAccess)
        {
            var tokenContext = new OpaqueTokenContext
            {
                TokenLength = _configurations.OpaqueTokenLength,
                TokenType = OpaqueTokenType.RefreshToken,
                ClientId = client.Id,
                Audiences = client.Audiences.Select(x => x.Audience).ToList(),
                Scopes = client.AllowedScopes.Select(x => x.Scope.Value).ToList(),
                ExpiredAt = DateTimeOffset.UtcNow.Add(client.RefreshTokenLifetime),
                Subject = user.Id.ToString(),
            };
            
            var tokenFactory = _tokenFactoryProvider.GetFactory<OpaqueTokenContext, string>();
            response.RefreshToken = await tokenFactory.CreateTokenAsync(tokenContext, cancellationToken);
        }

        return Results.Ok(response);
    }
}