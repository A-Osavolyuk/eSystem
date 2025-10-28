using eAccount.Application.State;
using eAccount.Domain.Constants;
using eAccount.Domain.Options;
using eAccount.Infrastructure.Security.Authentication.JWT;
using eAccount.Infrastructure.Security.Authentication.SSO.Clients;
using eAccount.Infrastructure.Utilities;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Authentication.SSO.Constants;
using eSystem.Core.Security.Cryptography.Keys;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using SignInRequest = eAccount.Domain.Requests.SignInRequest;
using SignInResponse = eAccount.Domain.Responses.SignInResponse;

namespace eAccount.Infrastructure.Security.Authentication;

public class AuthenticationManager(
    AuthenticationStateProvider authenticationStateProvider,
    UserState userState,
    TokenProvider tokenProvider,
    NavigationManager navigationManager,
    IFetchClient fetchClient,
    IKeyFactory keyFactory,
    IStorage storage,
    IOptions<ClientOptions> options)
{
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;
    private readonly UserState userState = userState;
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly NavigationManager navigationManager = navigationManager;
    private readonly IFetchClient fetchClient = fetchClient;
    private readonly IKeyFactory keyFactory = keyFactory;
    private readonly IStorage storage = storage;
    private readonly IOptions<ClientOptions> options = options;

    public async Task SignInAsync(string accessToken, string refreshToken)
    {
        tokenProvider.AccessToken = accessToken;

        var request = new SignInRequest()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        var fetchOptions = new FetchOptions()
        {
            Url = $"{navigationManager.BaseUri}api/authentication/sign-in",
            Method = HttpMethod.Post,
            Body = request
        };

        var result = await fetchClient.FetchAsync(fetchOptions);
        var response = result.Get<SignInResponse>()!;

        var identity = response.Identity;
        var claims = identity.Claims
            .Select(x => new Claim(x.Key, x.Value))
            .ToList();

        var claimsIdentity = new ClaimsIdentity(claims, identity.Scheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignIn(claimsPrincipal);
    }

    public void Authorize()
    {
        var queryParams = QueryHelper.GetQueryParameters(navigationManager.Uri);
        if (!queryParams.ContainsKey("client_id") && !queryParams.ContainsKey("redirect_uri"))
        {
            var clientOptions = options.Value;
            var state = keyFactory.Create(16);
            var nonce = keyFactory.Create(16);

            var builder = QueryBuilder.Create();
            builder
                .WithUri(Links.Authorize)
                .WithQueryParam("response_type", ResponseTypes.Code)
                .WithQueryParam("client_id", clientOptions.ClientId)
                .WithQueryParam("redirect_uri", clientOptions.RedirectUri)
                .WithQueryParam("scope", string.Join(' ', clientOptions.Scopes))
                .WithQueryParam("state", state)
                .WithQueryParam("nonce", nonce);

            if (queryParams.TryGetValue("return_url", out var returnUrl))
                builder.WithQueryParam("return_url", returnUrl);

            navigationManager.NavigateTo(builder.Build());
        }
        else
        {
            var responseType = queryParams["response_type"];
            var clientId = queryParams["client_id"];
            var redirectUri = queryParams["redirect_uri"];
            var scope = queryParams["scope"];
            var state = queryParams["state"];
            var nonce = queryParams["nonce"];

            var builder = QueryBuilder.Create();
            builder
                .WithUri(Links.Authorize)
                .WithQueryParam("response_type", responseType)
                .WithQueryParam("client_id", clientId)
                .WithQueryParam("redirect_uri", redirectUri)
                .WithQueryParam("scope", scope)
                .WithQueryParam("state", state)
                .WithQueryParam("nonce", nonce);

            if (queryParams.TryGetValue("code_challenge", out var codeChallenge)
                && queryParams.TryGetValue("code_challenge_method", out var codeChallengeMethod))
            {
                builder.WithQueryParam("code_challenge", codeChallenge)
                    .WithQueryParam("code_challenge_method", codeChallengeMethod);
            }

            navigationManager.NavigateTo(builder.Build());
        }
    }

    public async Task SignOutAsync()
    {
        var fetchOptions = new FetchOptions()
        {
            Method = HttpMethod.Post,
            Url = $"{navigationManager.BaseUri}api/authentication/sign-out",
        };

        var authResult = await fetchClient.FetchAsync(fetchOptions);
        if (authResult.Success)
        {
            await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignOutAsync();

            await storage.ClearAsync();
            tokenProvider.Clear();

            navigationManager.NavigateTo(Links.LoginPage);
        }
    }

    public async Task<HttpResponse> RefreshTokenAsync()
    {
        var request = new RefreshTokenRequest() { UserId = userState.UserId };
        var fetchOptions = new FetchOptions()
        {
            Method = HttpMethod.Post,
            Url = $"{navigationManager.BaseUri}api/sso/refresh-token",
            Body = request
        };

        return await fetchClient.FetchAsync(fetchOptions);
    }
}