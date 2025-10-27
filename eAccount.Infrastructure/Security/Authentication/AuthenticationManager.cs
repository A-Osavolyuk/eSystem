using eAccount.Application.State;
using eAccount.Domain.Constants;
using eAccount.Domain.Options;
using eAccount.Domain.Requests;
using eAccount.Infrastructure.Security.Authentication.JWT;
using eAccount.Infrastructure.Security.Authentication.SSO.Clients;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;
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
    IKeyFactory keyFactory,
    IFetchClient fetchClient,
    IStorage storage,
    IOptions<ClientOptions> options)
{
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;
    private readonly UserState userState = userState;
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly NavigationManager navigationManager = navigationManager;
    private readonly IKeyFactory keyFactory = keyFactory;
    private readonly IFetchClient fetchClient = fetchClient;
    private readonly IStorage storage = storage;
    private readonly ClientOptions options = options.Value;

    public async Task SignInAsync()
    {
        var nonce = keyFactory.Create(16);
        var state = keyFactory.Create(16);

        var authorizeRequest = new AuthorizeRequest()
        {
            UserId = userState.UserId,
            ClientId = options.ClientId,
            RedirectUri = options.RedirectUri,
            Scopes = options.Scopes,
            State = state,
            Nonce = nonce,
            ResponseType = ResponseTypes.Code
        };

        var authorizeResult = await AuthorizeAsync(authorizeRequest);
        if (!authorizeResult.Success)
        {
            
        }

        var authorizeResponse = authorizeResult.Get<AuthorizeResponse>()!;

        var tokenRequest = new TokenRequest()
        {
            GrantType = GrantTypes.AuthorizationCode,
            Code = authorizeResponse.Code,
            ClientId = options.ClientId,
            ClientSecret = options.ClientSecret,
            RedirectUri = options.RedirectUri
        };

        var tokenResult = await TokenAsync(tokenRequest);

        if (!tokenResult.Success)
        {
            
        }
        
        var tokenResponse = tokenResult.Get<TokenResponse>()!;

        tokenProvider.AccessToken = tokenResponse.AccessToken;

        var signInRequest = new SignInRequest()
        {
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken
        };

        var signInResult = await SignInAsync(signInRequest);

        if (!signInResult.Success)
        {

        }
        
        var signInResponse = signInResult.Get<SignInResponse>()!;

        var identity = signInResponse.Identity;
        var claims = identity.Claims
            .Select(x => new Claim(x.Key, x.Value))
            .ToList();

        var claimsIdentity = new ClaimsIdentity(claims, identity.Scheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignIn(claimsPrincipal);
    }

    private async Task<HttpResponse> SignInAsync(SignInRequest request)
    {
        var fetchOptions = new FetchOptions()
        {
            Url = $"{navigationManager.BaseUri}api/authentication/sign-in",
            Method = HttpMethod.Post,
            Body = request
        };

        return await fetchClient.FetchAsync(fetchOptions);
    }

    private async Task<HttpResponse> AuthorizeAsync(AuthorizeRequest request)
    {
        var fetchOptions = new FetchOptions()
        {
            Url = $"{navigationManager.BaseUri}api/sso/authorize",
            Method = HttpMethod.Post,
            Body = request
        };

        return await fetchClient.FetchAsync(fetchOptions);
    }

    private async Task<HttpResponse> TokenAsync(TokenRequest request)
    {
        var fetchOptions = new FetchOptions()
        {
            Url = $"{navigationManager.BaseUri}api/sso/token",
            Method = HttpMethod.Post,
            Body = request
        };

        return await fetchClient.FetchAsync(fetchOptions);
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