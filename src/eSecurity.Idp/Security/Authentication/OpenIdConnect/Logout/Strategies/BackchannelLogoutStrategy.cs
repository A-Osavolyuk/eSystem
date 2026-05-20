using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Idp.Security.Cryptography.Tokens;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Server.Encoding;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.Logout;

namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Logout.Strategies;

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
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "User was not found"
            });
        }

        var clients = await _clientManager.GetClientsAsync(session, cancellationToken);
        foreach (var client in clients.Where(x => x.AllowBackChannelLogout))
        {
            var backchannelLogoutUri = client.Uris.FirstOrDefault(x => x.Type == UriType.BackChannelLogout);
            if (backchannelLogoutUri is null) continue;

            var accessTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.LogoutToken);
            var accessTokenResult = await accessTokenFactory.CreateAsync(client, user, 
                session, cancellationToken: cancellationToken);

            if (!accessTokenResult.Succeeded)
            {
                var error = accessTokenResult.GetError();
                return Results.ServerError(ServerErrorCode.InternalServerError, error);
            }

            if (!accessTokenResult.TryGetValue(out var token))
            {
                return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
                {
                    Code = ErrorCode.ServerError,
                    Description = "Server error"
                });
            }
            
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, backchannelLogoutUri.Uri)
            {
                Content = new FormUrlEncodedContent(FormUrl.Encode(
                    new BackChannelLogoutRequest { LogoutToken = token }))
            };

            await _httpClient.SendAsync(requestMessage, cancellationToken);
        }

        return Results.Success(SuccessCodes.Ok);
    }
}