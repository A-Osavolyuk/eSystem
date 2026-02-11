using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.Claims.Factories;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Logout;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Cryptography.Encoding;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Logout.Strategies;

public class BackchannelLogoutStrategy(
    IHttpClientFactory httpClientFactory,
    IClientManager clientManager,
    IUserManager userManager,
    IClaimFactoryProvider claimFactoryProvider,
    ITokenBuilderProvider tokenBuilderProvider,
    IOptions<TokenConfigurations> options) : ILogoutStrategy<Result>
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly IUserManager _userManager = userManager;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("BackchannelLogoutHandler");

    private readonly ITokenClaimsFactory<LogoutTokenClaimsContext, UserEntity> _claimsFactory =
        claimFactoryProvider.GetClaimFactory<LogoutTokenClaimsContext, UserEntity>();

    private readonly ITokenBuilder<JwtTokenBuildContext, string> _tokenBuilder =
        tokenBuilderProvider.GetFactory<JwtTokenBuildContext, string>();

    public async ValueTask<Result> ExecuteAsync(SessionEntity session, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(session.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User was not found");

        var clients = await _clientManager.GetClientsAsync(session, cancellationToken);
        foreach (var client in clients.Where(x => x.AllowBackChannelLogout))
        {
            var backchannelLogoutUri = client.Uris.FirstOrDefault(x => x.Type == UriType.BackChannelLogout);
            if (backchannelLogoutUri is null) continue;

            var lifetime = client.LogoutTokenLifetime ?? _tokenConfigurations.DefaultLogoutTokenLifetime;
            var claimsContext = new LogoutTokenClaimsContext()
            {
                Aud = client.Id.ToString(),
                Sid = session.Id.ToString(),
                Exp = DateTimeOffset.UtcNow.Add(lifetime)
            };

            var claims = await _claimsFactory.GetClaimsAsync(user, claimsContext, cancellationToken);
            var tokenContext = new JwtTokenBuildContext { Claims = claims, Type = JwtTokenTypes.Generic };
            var token = await _tokenBuilder.BuildAsync(tokenContext, cancellationToken);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, backchannelLogoutUri.Uri)
            {
                Content = new FormUrlEncodedContent(FormUrl.Encode(
                    new BackChannelLogoutRequest { LogoutToken = token }))
            };

            await _httpClient.SendAsync(requestMessage, cancellationToken);
        }

        return Results.Ok();
    }
}