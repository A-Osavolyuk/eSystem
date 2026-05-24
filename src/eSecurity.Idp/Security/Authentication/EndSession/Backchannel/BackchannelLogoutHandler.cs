using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Cryptography.Tokens;
using eSecurity.Idp.Security.Cryptography.Tokens.Logout;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Server.Encoding;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.Logout;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Idp.Security.Authentication.EndSession.Backchannel;

public sealed class BackchannelLogoutHandler(
    IOptions<EndSessionOptions> options,
    IClientManager clientManager,
    ITokenFactoryProvider tokenFactoryProvider,
    IHttpClientFactory httpClientFactory) : IBackchannelLogoutHandler
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly EndSessionOptions _options = options.Value;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("BackchannelLogoutHandler");

    public async ValueTask<Result> HandleAsync(EndSessionRequestEntity request, 
        CancellationToken cancellationToken = default)
    {
        var fallbackUri = _options.FallbackUrl;
        if (string.IsNullOrEmpty(fallbackUri))
            throw new InvalidOperationException("End session flow was not configured correctly");
        
        var clients = await _clientManager.GetClientsAsync(request.Session, cancellationToken);
        foreach (var client in clients.Where(x => x.AllowBackChannelLogout))
        {
            var backchannelLogoutUri = client.Uris.FirstOrDefault(x => x.Type == UriType.BackChannelLogout);
            if (backchannelLogoutUri is null) 
                continue;

            var logoutTokenFactoryContext = new LogoutTokenFactoryContext
            {
                Client = client,
                User = request.User,
                Session = request.Session
            };
                
            var logoutTokenFactory = _tokenFactoryProvider.GetFactory<LogoutTokenFactoryContext>();
            var logoutTokenResult = await logoutTokenFactory.CreateAsync(logoutTokenFactoryContext, 
                cancellationToken: cancellationToken);

            if (!logoutTokenResult.Succeeded)
            {
                var error = logoutTokenResult.GetError();
                return Fallback(fallbackUri, error.Code, error.Description, request.State);
            }

            if (!logoutTokenResult.TryGetValue(out var token))
                return Fallback(fallbackUri, ErrorCode.ServerError, "Server error", request.State);
            
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, backchannelLogoutUri.Uri)
            {
                Content = new FormUrlEncodedContent(FormUrl.Encode(
                    new BackChannelLogoutRequest { LogoutToken = token }))
            };

            await _httpClient.SendAsync(requestMessage, cancellationToken);
        }

        return Results.Success(SuccessCodes.Ok);
    }
    
    private static Result Fallback(string uri, ErrorCode error, string description, string? state = null)
    {
        var builder = QueryBuilder.Create()
            .WithUri(uri)
            .WithQueryParam("error", error)
            .WithQueryParam("error_description", description);

        if (!string.IsNullOrEmpty(state))
            builder.WithQueryParam("state", state);

        return Results.Redirect(RedirectionCode.Found, builder.Build());
    }
}