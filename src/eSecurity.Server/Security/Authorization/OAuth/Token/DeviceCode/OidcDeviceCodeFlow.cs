using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
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
    IClaimFactoryProvider claimFactoryProvider,
    ITokenBuilderProvider tokenBuilderProvider,
    IUserManager userManager,
    ISessionManager sessionManager,
    IOptions<TokenConfigurations> tokenOptions) : IDeviceCodeFlow
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly IDeviceCodeManager _deviceCodeManager = deviceCodeManager;
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;
    private readonly IUserManager _userManager = userManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly TokenConfigurations _tokenConfigurations = tokenOptions.Value;

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
        deviceCode.ConsumedAt = DateTimeOffset.UtcNow;
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
            ExpiresIn = (int)_tokenConfigurations.DefaultAccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenTypes.Bearer,
        };
        
        var session = await _sessionManager.FindAsync(user, cancellationToken);
        if (session is null || session.ExpireDate < DateTimeOffset.UtcNow)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        if (client.AccessTokenType == AccessTokenType.Jwt)
        {
            var lifetime = client.AccessTokenLifetime ?? _tokenConfigurations.DefaultAccessTokenLifetime;
            var claimsFactory = _claimFactoryProvider.GetClaimFactory<AccessTokenClaimsContext, UserEntity>();
            var claims = await claimsFactory.GetClaimsAsync(user, new AccessTokenClaimsContext
            {
                Exp = DateTimeOffset.UtcNow.Add(lifetime),
                Aud = client.Audiences.Select(x => x.Audience),
                Scopes = client.AllowedScopes.Select(x => x.Scope.Value),
            }, cancellationToken);

            var tokenContext = new JwtTokenBuildContext { Claims = claims, Type = JwtTokenTypes.AccessToken };
            var tokenFactory = _tokenBuilderProvider.GetFactory<JwtTokenBuildContext, string>();

            response.AccessToken = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
        }
        else
        {
            var lifetime = client.AccessTokenLifetime ?? _tokenConfigurations.DefaultAccessTokenLifetime;
            var tokenContext = new OpaqueTokenBuildContext
            {
                TokenLength = _tokenConfigurations.OpaqueTokenLength,
                TokenType = OpaqueTokenType.AccessToken,
                ClientId = client.Id,
                Audiences = client.Audiences.Select(x => x.Audience).ToList(),
                Scopes = client.AllowedScopes.Select(x => x.Scope.Value).ToList(),
                ExpiredAt = DateTimeOffset.UtcNow.Add(lifetime),
                Subject = user.Id.ToString(),
                Sid = session.Id
            };
            
            var tokenFactory = _tokenBuilderProvider.GetFactory<OpaqueTokenBuildContext, string>();
            response.AccessToken = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
        }

        var clientResult = await _clientManager.RelateAsync(client, session, cancellationToken);
        if (!clientResult.Succeeded) return clientResult;

        if (client.AllowOfflineAccess && client.HasScope(ScopeTypes.OfflineAccess))
        {
            var lifetime = client.RefreshTokenLifetime ?? _tokenConfigurations.DefaultRefreshTokenLifetime;
            var tokenContext = new OpaqueTokenBuildContext
            {
                TokenLength = _tokenConfigurations.OpaqueTokenLength,
                TokenType = OpaqueTokenType.RefreshToken,
                ClientId = client.Id,
                Audiences = client.Audiences.Select(x => x.Audience).ToList(),
                Scopes = client.AllowedScopes.Select(x => x.Scope.Value).ToList(),
                ExpiredAt = DateTimeOffset.UtcNow.Add(lifetime),
                Subject = user.Id.ToString(),
                Sid = session.Id
            };
            var refreshTokenFactory = _tokenBuilderProvider.GetFactory<OpaqueTokenBuildContext, string>();
            response.RefreshToken = await refreshTokenFactory.BuildAsync(tokenContext, cancellationToken);
        }
        
        if (client.HasGrantType(GrantTypes.Ciba))
        {
            var lifetime = client.LoginTokenLifetime ?? _tokenConfigurations.DefaultLoginTokenLifetime;
            var tokenContext = new OpaqueTokenBuildContext
            {
                TokenLength = _tokenConfigurations.OpaqueTokenLength,
                TokenType = OpaqueTokenType.LoginToken,
                Sid = session.Id,
                ClientId = client.Id,
                ExpiredAt = DateTimeOffset.UtcNow.Add(lifetime),
                Subject = user.Id.ToString(),
            };
            
            var tokenFactory = _tokenBuilderProvider.GetFactory<OpaqueTokenBuildContext, string>();
            response.LoginTokenHint = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
        }

        var idTokenLifetime = client.IdTokenLifetime ?? _tokenConfigurations.DefaultIdTokenLifetime;
        var idClaimsFactory = _claimFactoryProvider.GetClaimFactory<IdTokenClaimsContext, UserEntity>();
        var idClaims = await idClaimsFactory.GetClaimsAsync(user, new IdTokenClaimsContext
        {
            Aud = client.Id.ToString(),
            Scopes = client.AllowedScopes.Select(x => x.Scope.Value),
            Sid = session.Id.ToString(),
            AuthenticationMethods = session.AuthenticationMethods,
            AuthTime = DateTimeOffset.UtcNow,
            Exp = DateTimeOffset.UtcNow.Add(idTokenLifetime)
        }, cancellationToken);

        var idTokenContext = new JwtTokenBuildContext { Claims = idClaims, Type = JwtTokenTypes.IdToken };
        var idTokenFactory = _tokenBuilderProvider.GetFactory<JwtTokenBuildContext, string>();

        response.IdToken = await idTokenFactory.BuildAsync(idTokenContext, cancellationToken);
        return Results.Ok(response);
    }
}