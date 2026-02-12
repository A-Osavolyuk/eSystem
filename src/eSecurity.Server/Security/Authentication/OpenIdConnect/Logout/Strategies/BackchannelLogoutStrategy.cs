using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Logout;
using eSystem.Core.Security.Cryptography.Encoding;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Logout.Strategies;

public class BackchannelLogoutStrategy(
    IHttpClientFactory httpClientFactory,
    IClientManager clientManager,
    IUserManager userManager,
    ITokenFactoryProvider tokenFactoryProvider,
    IOptions<TokenConfigurations> options) : ILogoutStrategy<Result>
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly IUserManager _userManager = userManager;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("BackchannelLogoutHandler");

    public async ValueTask<Result> ExecuteAsync(SessionEntity session, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(session.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User was not found");

        var clients = await _clientManager.GetClientsAsync(session, cancellationToken);
        foreach (var client in clients.Where(x => x.AllowBackChannelLogout))
        {
            var backchannelLogoutUri = client.Uris.FirstOrDefault(x => x.Type == UriType.BackChannelLogout);
            if (backchannelLogoutUri is null) continue;

            var accessTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.LogoutToken);
            var accessTokenResult = await accessTokenFactory.CreateAsync(client, user, 
                session, cancellationToken: cancellationToken);
        
            if (!accessTokenResult.IsSucceeded) 
                return Results.InternalServerError(accessTokenResult.Error!);
        
            var token = accessTokenResult.Token!;
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