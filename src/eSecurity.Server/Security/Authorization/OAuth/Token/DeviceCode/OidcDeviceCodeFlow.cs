using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.Claims.Factories;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token.DeviceCode;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;

public sealed class OidcDeviceCodeFlow(
    IClientManager clientManager,
    IDeviceCodeManager deviceCodeManager,
    ITokenManager tokenManager,
    IClaimFactoryProvider claimFactoryProvider,
    ITokenFactoryProvider tokenFactoryProvider,
    IHasherProvider hasherProvider,
    IUserManager userManager,
    ISessionManager sessionManager,
    IOptions<TokenOptions> tokenOptions) : IDeviceCodeFlow
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly IDeviceCodeManager _deviceCodeManager = deviceCodeManager;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly IUserManager _userManager = userManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly TokenOptions _tokenOptions = tokenOptions.Value;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Sha512);

    public async ValueTask<Result> ExecuteAsync(DeviceCodeEntity deviceCode, DeviceCodeFlowContext context,
        CancellationToken cancellationToken = default)
    {
        var client = await _clientManager.FindByIdAsync(context.ClientId, cancellationToken);
        if (client is null || deviceCode.UserId is null || deviceCode.SessionId is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid device code"
            });
        }

        deviceCode.State = DeviceCodeState.Consumed;
        deviceCode.IsFirstPoll = false;

        var deviceResult = await _deviceCodeManager.UpdateAsync(deviceCode, cancellationToken);
        if (deviceResult.Succeeded) return deviceResult;

        var user = await _userManager.FindByIdAsync(deviceCode.UserId.Value, cancellationToken);
        if (user is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid device code"
            });
        }

        var response = new DeviceCodeResponse
        {
            ExpiresIn = (int)_tokenOptions.AccessTokenLifetime.TotalSeconds,
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
            var tokenContext = new OpaqueTokenContext { Length = _tokenOptions.RefreshTokenLength };
            var tokenFactory = _tokenFactoryProvider.GetFactory<OpaqueTokenContext, string>();
            var rawToken = await tokenFactory.CreateTokenAsync(tokenContext, cancellationToken);
            var accessToken = new OpaqueTokenEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = client.Id,
                Subject = user.Id.ToString(),
                TokenHash = _hasher.Hash(rawToken),
                TokenType = OpaqueTokenType.AccessToken,
                ExpiredAt = DateTimeOffset.UtcNow.Add(_tokenOptions.AccessTokenLifetime)
            };
            
            accessToken.Audiences = client.Audiences.Select(x => new OpaqueTokenAudienceEntity()
            {
                Id = Guid.CreateVersion7(),
                TokenId = accessToken.Id,
                AudienceId = x.Id
            }).ToList();
            
            accessToken.Scopes = client.AllowedScopes.Select(x => new OpaqueTokenScopeEntity()
            {
                Id = Guid.CreateVersion7(),
                TokenId = accessToken.Id,
                ScopeId = x.Id
            }).ToList();

            var createResult = await _tokenManager.CreateAsync(accessToken, cancellationToken);
            if (!createResult.Succeeded) return createResult;

            response.AccessToken = rawToken;
        }

        var session = await _sessionManager.FindAsync(user, cancellationToken);
        if (session is null || session.ExpireDate < DateTimeOffset.UtcNow)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        var clientResult = await _clientManager.RelateAsync(client, session, cancellationToken);
        if (!clientResult.Succeeded) return clientResult;

        if (client.AllowOfflineAccess && client.HasScope(ScopeTypes.OfflineAccess))
        {
            var refreshTokenContext = new OpaqueTokenContext { Length = _tokenOptions.RefreshTokenLength };
            var refreshTokenFactory = _tokenFactoryProvider.GetFactory<OpaqueTokenContext, string>();
            var rawToken = await refreshTokenFactory.CreateTokenAsync(refreshTokenContext, cancellationToken);
            var refreshToken = new OpaqueTokenEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = client.Id,
                SessionId = session.Id,
                Subject = user.Id.ToString(),
                TokenHash = _hasher.Hash(rawToken),
                TokenType = OpaqueTokenType.RefreshToken,
                ExpiredAt = DateTimeOffset.UtcNow.Add(client.RefreshTokenLifetime)
            };
            
            refreshToken.Audiences = client.Audiences.Select(x => new OpaqueTokenAudienceEntity()
            {
                Id = Guid.CreateVersion7(),
                TokenId = refreshToken.Id,
                AudienceId = x.Id
            }).ToList();
            
            refreshToken.Scopes = client.AllowedScopes.Select(x => new OpaqueTokenScopeEntity()
            {
                Id = Guid.CreateVersion7(),
                TokenId = refreshToken.Id,
                ScopeId = x.Id
            }).ToList();

            refreshToken.Audiences = client.Audiences.Select(x => new OpaqueTokenAudienceEntity()
            {
                Id = Guid.CreateVersion7(),
                TokenId = refreshToken.Id,
                AudienceId = x.Id
            }).ToList();

            var tokenResult = await _tokenManager.CreateAsync(refreshToken, cancellationToken);
            if (!tokenResult.Succeeded) return tokenResult;

            response.RefreshToken = rawToken;
        }

        var idClaimsFactory = _claimFactoryProvider.GetClaimFactory<IdTokenClaimsContext, UserEntity>();
        var idClaims = await idClaimsFactory.GetClaimsAsync(user, new IdTokenClaimsContext
        {
            Aud = client.Id.ToString(),
            Scopes = client.AllowedScopes.Select(x => x.Scope.Value),
            Sid = session.Id.ToString(),
            AuthenticationMethods = session.AuthenticationMethods,
            AuthTime = DateTimeOffset.UtcNow,
        }, cancellationToken);

        var idTokenContext = new JwtTokenContext { Claims = idClaims, Type = JwtTokenTypes.IdToken };
        var idTokenFactory = _tokenFactoryProvider.GetFactory<JwtTokenContext, string>();

        response.IdToken = await idTokenFactory.CreateTokenAsync(idTokenContext, cancellationToken);
        return Results.Ok(response);
    }
}