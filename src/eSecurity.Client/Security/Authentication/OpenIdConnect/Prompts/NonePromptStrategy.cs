using System.Web;
using eSecurity.Client.Common.JS.Fetch;
using eSecurity.Client.Common.State;
using eSecurity.Client.Common.State.States;
using eSecurity.Client.Security.Authentication.OpenIdConnect.Authorization;
using eSecurity.Client.Security.Cookies;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Utilities.Query;
using Microsoft.AspNetCore.Components;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Prompts;

public sealed class NonePromptStrategy(
    UserState userState,
    NavigationManager navigationManager,
    IConnectService connectService,
    IFetchClient fetchClient) : IPromptStrategy
{
    private readonly UserState _userState = userState;
    private readonly NavigationManager _navigationManager = navigationManager;
    private readonly IConnectService _connectService = connectService;
    private readonly IFetchClient _fetchClient = fetchClient;

    public bool CanHandle(string? prompt) => prompt == PromptTypes.None || string.IsNullOrEmpty(prompt);

    public async Task<AuthorizationResult> ExecuteAsync(AuthorizationContext context)
    {
        var decodedRedirectUri = HttpUtility.UrlDecode(context.RedirectUri);
        var decodedScope = HttpUtility.UrlDecode(context.Scope);
        
        if (!_userState.IsAuthenticated)
        {
            var redirectUri = QueryBuilder.Create()
                .WithUri(decodedRedirectUri)
                .WithQueryParam("error", ErrorTypes.OAuth.LoginRequired)
                .WithQueryParam("error_description", "User must be authenticated.")
                .Build();
            
            return AuthorizationResult.Redirect(redirectUri);
        }
        
        var request = new AuthorizeRequest
        {
            UserId = _userState.UserId,
            ResponseType = context.ResponseType,
            ClientId = context.ClientId,
            RedirectUri = decodedRedirectUri,
            Scopes = decodedScope.Split(' ').ToList(),
            Nonce = context.Nonce,
            State = context.State,
            CodeChallenge = context.CodeChallenge,
            CodeChallengeMethod = context.CodeChallengeMethod
        };

        var result = await _connectService.AuthorizeAsync(request);
        if (result.Succeeded && result.TryGetValue<AuthorizeResponse>(out var response))
        {
            var session = new SessionCookie
            {
                Id = response!.SessionId,
                UserId = request.UserId,
                DeviceId = response.DeviceId,
                IssuedAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(30)
            };

            var fetchOptions = new FetchOptions
            {
                Url = $"{_navigationManager.BaseUri}api/authentication/authorize",
                Method = HttpMethod.Post,
                Body = session
            };

            await _fetchClient.FetchAsync(fetchOptions);

            var builder = QueryBuilder.Create()
                .WithUri(decodedRedirectUri)
                .WithQueryParam("code", response.Code)
                .WithQueryParam("state", response.State);

            if (!string.IsNullOrEmpty(context.ReturnUrl)) 
                builder.WithQueryParam("return_url", context.ReturnUrl);

            return AuthorizationResult.Redirect(builder.Build());
        }
        else
        {
            var error = result.GetError();
            var redirectUri = QueryBuilder.Create()
                .WithUri(context.RedirectUri)
                .WithQueryParam("error", error.Code)
                .WithQueryParam("error_description", error.Description)
                .Build();
            
            return AuthorizationResult.Redirect(redirectUri);
        }
    }
}