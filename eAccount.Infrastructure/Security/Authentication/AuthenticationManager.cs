using eAccount.Application.State;
using eAccount.Domain.Options;
using eAccount.Domain.Requests;
using eAccount.Infrastructure.Security.Authentication.JWT;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Authentication.SSO.Constants;
using eSystem.Core.Security.Cryptography.Keys;
using Microsoft.AspNetCore.Components;
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
    IStorage storage)
{
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;
    private readonly UserState userState = userState;
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly NavigationManager navigationManager = navigationManager;
    private readonly IKeyFactory keyFactory = keyFactory;
    private readonly IFetchClient fetchClient = fetchClient;
    private readonly IStorage storage = storage;

    public async Task SignInAsync()
    {
        var clientId = "eAccount";
        var clientSecret = "2f213a036e325a55dc19320f03c2fad7c13f0169788b5968686cb4931341c393a651d7e6";
        var redirectUri = "none";
        var nonce = keyFactory.Create(16);
        var state = keyFactory.Create(16);
        var scopes = new List<string>()
        {
            Scopes.OpenId,
            Scopes.Profile,
            Scopes.Email,
            Scopes.PhoneNumber,
            Scopes.Address
        };
        
        var authorizeRequest = new AuthorizeRequest()
        {
            UserId = userState.UserId,
            ClientId = clientId,
            RedirectUri = redirectUri,
            Scopes = scopes,
            State = state,
            Nonce = nonce,
            ResponseType = ResponseTypes.Code
        };
        
        var authorizeResult = await AuthorizeAsync(authorizeRequest);
        if (authorizeResult.Success)
        {
            var response = authorizeResult.Get<AuthorizeResponse>()!;

            if (!state.Equals(response.State))
            {
                //TODO: Handle invalid state
            }

            var tokenRequest = new TokenRequest()
            {
                GrantType = GrantTypes.AuthorizationCode,
                Code = response.Code,
                ClientId = clientId,
                ClientSecret = clientSecret,
                RedirectUri = redirectUri
            };
            
            var tokenResult = await TokenAsync(tokenRequest);

            if (tokenResult.Success)
            {
                var tokenResponse = tokenResult.Get<TokenResponse>()!;
                
                tokenProvider.AccessToken = tokenResponse.AccessToken;

                var signInRequest = new SignInRequest()
                {
                    AccessToken = tokenResponse.AccessToken,
                    RefreshToken = tokenResponse.RefreshToken
                };
                
                var signInResult = await SignInAsync(signInRequest);
                
                if (signInResult.Success)
                {
                    var signInResponse = signInResult.Get<SignInResponse>()!;

                    var identity = signInResponse.Identity;
                    var claims = identity.Claims
                        .Select(x => new Claim(x.Key, x.Value))
                        .ToList();
                    
                    var claimsIdentity = new ClaimsIdentity(claims, identity.Scheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignIn(claimsPrincipal);
                }
                //TODO: Handle sign-in failure
            }
            //TODO: Handle token failure
        }
        //TODO: Handle authorize failure
    }

    private async Task<HttpResponse> SignInAsync(SignInRequest request)
    {
        var fetchOptions = new FetchOptions()
        {
            Url = $"{navigationManager.BaseUri}api/auth/sign-in",
            Method = HttpMethod.Post,
            Body = request
        };

        return await fetchClient.FetchAsync(fetchOptions);
    }

    private async Task<HttpResponse> AuthorizeAsync(AuthorizeRequest request)
    {
        var fetchOptions = new FetchOptions()
        {
            Url = $"{navigationManager.BaseUri}api/auth/authorize",
            Method = HttpMethod.Post,
            Body = request
        };

        return await fetchClient.FetchAsync(fetchOptions);
    }
    
    private async Task<HttpResponse> TokenAsync(TokenRequest request)
    {
        var fetchOptions = new FetchOptions()
        {
            Url = $"{navigationManager.BaseUri}api/auth/token",
            Method = HttpMethod.Post,
            Body = request
        };

        return await fetchClient.FetchAsync(fetchOptions);
    }

    public async Task SignOutAsync()
    {
        var request = new SignOutRequest()
        {
            UserId = userState.UserId,
            AccessToken = tokenProvider.AccessToken!
        };
        
        var fetchOptions = new FetchOptions()
        {
            Method = HttpMethod.Post,
            Url = $"{navigationManager.BaseUri}api/auth/sign-out",
            Body = request
        };

        var authResult = await fetchClient.FetchAsync(fetchOptions);
        if (authResult.Success)
        {
            await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignOutAsync();

            await storage.ClearAsync();
            tokenProvider.Clear();

            navigationManager.NavigateTo("/account/login");
        }
    }

    public async Task<HttpResponse> RefreshTokenAsync()
    {
        var request = new RefreshTokenRequest() { UserId = userState.UserId };
        var options = new FetchOptions()
        {
            Method = HttpMethod.Post,
            Url = $"{navigationManager.BaseUri}api/auth/refresh-token",
            Body = request
        };

        return await fetchClient.FetchAsync(options);
    }
}