using ClaimTypes = eShop.Domain.Common.Security.ClaimTypes;
using UserModel = eShop.Domain.Models.UserModel;

namespace eShop.Infrastructure.Account;

public class ApplicationAuthenticationStateProvider(
    ITokenProvider tokenProvider,
    IAuthenticationService authenticationService,
    ILocalStorage localStorage,
    IUserStorage userStorage) : AuthenticationStateProvider
{
    private readonly AuthenticationState anonymous = new(new ClaimsPrincipal());
    private readonly ITokenProvider tokenProvider = tokenProvider;
    private readonly IAuthenticationService authenticationService = authenticationService;
    private readonly ILocalStorage localStorage = localStorage;
    private readonly IUserStorage userStorage = userStorage;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(AuthenticationHandler.Token))
            {
                return await Task.FromResult(anonymous);
            }

            var token = DecryptToken(AuthenticationHandler.Token);

            if (token is null || !token.Claims.Any())
            {
                return await Task.FromResult(anonymous);
            }

            var valid = IsValid(token);

            if (!valid)
            {
                await LogOutAsync();
                return await Task.FromResult(anonymous);
            }

            var claims = SetClaims(token);

            if (!claims.Any())
            {
                return await Task.FromResult(anonymous);
            }

            return await Task.FromResult(
                new AuthenticationState(new ClaimsPrincipal(
                    new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme))));
        }
        catch (Exception)
        {
            return await Task.FromResult(anonymous);
        }
    }

    public async Task UpdateAuthenticationStateAsync(string token)
    {
        var claimsPrincipal = new ClaimsPrincipal();

        if (string.IsNullOrEmpty(token))
        {
            AuthenticationHandler.Token = string.Empty;
        }
        else
        {
            AuthenticationHandler.Token = token;
            await tokenProvider.SetTokenAsync(token);

            var rawToken = DecryptToken(token)!;
            var claims = SetClaims(rawToken);
            await WriteToLocalStorageAsync(claims);
            claimsPrincipal = new(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme));
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    public async Task LoginAsync(string accessToken, string refreshToken)
    {
        var claimsPrincipal = new ClaimsPrincipal();

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
        {
            AuthenticationHandler.Token = string.Empty;
        }
        else
        {
            AuthenticationHandler.Token = refreshToken;
            await tokenProvider.SetTokenAsync(refreshToken);

            var rawToken = DecryptToken(accessToken)!;
            var claims = SetClaims(rawToken);
            await WriteToLocalStorageAsync(claims);
            claimsPrincipal = new(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme));
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    private JwtSecurityToken? DecryptToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!string.IsNullOrEmpty(token) && handler.CanReadToken(token))
        {
            var rawToken = handler.ReadJwtToken(token);
            return rawToken;
        }

        return new JwtSecurityToken();
    }

    private List<Claim> SetClaims(JwtSecurityToken token)
    {
        var claims = token.Claims.ToList();

        var output = new List<Claim>()
        {
            new(ClaimTypes.Id, claims.FirstOrDefault(x => x.Type == ClaimTypes.Id)!.Value),
            new(ClaimTypes.UserName, claims.FirstOrDefault(x => x.Type == ClaimTypes.UserName)!.Value),
            new(ClaimTypes.Email, claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)!.Value),
            new(ClaimTypes.PhoneNumber, claims.FirstOrDefault(x => x.Type == ClaimTypes.PhoneNumber)!.Value),
        };

        var roles = claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
        var permissions = claims.Where(x => x.Type == ClaimTypes.Permission).Select(x => x.Value).ToList();

        if (roles.Any())
        {
            foreach (var role in roles)
            {
                output.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        if (permissions.Any())
        {
            foreach (var permission in permissions)
            {
                output.Add(new Claim(ClaimTypes.Permission, permission));
            }
        }

        return output;
    }

    private async Task WriteToLocalStorageAsync(List<Claim> claims)
    {
        var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)!.Value;
        var username = claims.FirstOrDefault(x => x.Type == ClaimTypes.UserName)!.Value;
        var phoneNumber = claims.FirstOrDefault(x => x.Type == ClaimTypes.PhoneNumber)!.Value;
        var id = claims.FirstOrDefault(x => x.Type == ClaimTypes.Id)!.Value;
        
        //TODO: Implement storing roles and permissions in local storage
        
        var roles = claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
        var permissions = claims.Where(x => x.Type == ClaimTypes.Permission).Select(x => x.Value).ToList();

        await userStorage.SetUserAsync(new UserModel()
        {
            AccountData = new ()
            {
                Email = email,
                UserName = username,
                PhoneNumber = phoneNumber,
                Id = Guid.Parse(id),
            }
        });
    }

    private bool IsValid(JwtSecurityToken token)
    {
        var expValue = token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)!.Value;
        var expMilliseconds = Convert.ToInt64(expValue);
        var expData = DateTimeOffset.FromUnixTimeSeconds(expMilliseconds);

        return DateTimeOffset.Now < expData;
    }

    private async Task<AuthenticationState> RefreshTokenAsync(string expiredToken)
    {
        var result = await authenticationService.RefreshToken(new RefreshTokenRequest() { Token = expiredToken });

        if (result.Success)
        {
            var response = JsonConvert.DeserializeObject<RefreshTokenResponse>(result.Result!.ToString()!)!;
            var newToken = response.Token;

            if (!string.IsNullOrEmpty(newToken))
            {
                var token = DecryptToken(newToken);

                if (token is not null || token!.Claims.Any())
                {
                    var claims = SetClaims(token);

                    if (claims.Any())
                    {
                        AuthenticationHandler.Token = newToken;
                        return await Task.FromResult(
                            new AuthenticationState(new ClaimsPrincipal(
                                new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme))));
                    }
                }
            }
        }

        return await Task.FromResult(anonymous);
    }

    public async Task LogOutAsync()
    {
        await tokenProvider.ClearAsync();
        await localStorage.ClearAsync();
        await userStorage.ClearAsync();
        await UpdateAuthenticationStateAsync(string.Empty);
    }
}