using eSecurity.Server.Security.Authentication.OpenIdConnect.Discovery;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Token;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Validation;
using eSecurity.Server.Security.Authentication.Ticket;
using eSecurity.Server.Security.Cookies;
using eSecurity.Server.Security.Identity;
using eSystem.Core.Configuration;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Identity.Claims;
using eSystem.Core.Server.Security.Authorization.OAuth;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.Validation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace eSecurity.Server.Security.Authentication;

using TokenHandler = OpenIdConnect.Token.TokenHandler;

public static class AuthenticationExtensions
{
    public static void AddAuthentication(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITokenHandler, TokenHandler>();
        builder.Services.AddScoped<ISessionManager, SessionManager>();
        builder.Services.AddScoped<ITokenValidator, LogoutTokenValidator>();
        builder.Services.AddScoped<IOpenIdDiscoveryProvider, OpenIdDiscoveryProvider>();
        builder.Services.AddSingleton<IUserIdProvider, SubjectUserIdProvider>();

        builder.Services.Configure<OAuthOptions>(builder.Configuration.GetSection("OAuth"));
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = DefaultCookies.State;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                options.SlidingExpiration = true;
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                var gatewayUrl = builder.Configuration.GetValue<string>("PROXY_HTTPS");
                var oauthOptions = builder.Configuration.Get<OAuthOptions>("OAuth");

                options.Authority = $"{gatewayUrl}{oauthOptions.Authority}";
                options.ClientId = oauthOptions.ClientId;
                options.ClientSecret = oauthOptions.ClientSecret;
                options.MapInboundClaims = oauthOptions.MapInboundClaims;
                options.GetClaimsFromUserInfoEndpoint = oauthOptions.GetClaimsFromUserInfoEndpoint;
                options.SaveTokens = oauthOptions.SaveTokens;
                options.CallbackPath = oauthOptions.CallbackPath;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                options.SignedOutCallbackPath = oauthOptions.SignedOutCallbackPath;
                options.SignedOutRedirectUri = oauthOptions.SignedOutRedirectUri;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = AppClaimTypes.PreferredUsername,
                    RoleClaimType = AppClaimTypes.Role
                };

                options.ResponseType = oauthOptions.ResponseType;
                options.Prompt = oauthOptions.Prompt;
                options.UsePkce = oauthOptions.UsePkce;

                options.Scope.Clear();
                options.Scope.Add(ScopeTypes.OpenId);
                options.Scope.Add(ScopeTypes.OfflineAccess);
                options.Scope.Add(ScopeTypes.Email);
                options.Scope.Add(ScopeTypes.Phone);
                options.Scope.Add(ScopeTypes.Profile);
                options.Scope.Add(ScopeTypes.Address);

                options.Events = new OpenIdConnectEvents
                {
                    OnRemoteFailure = ctx =>
                    {
                        ctx.Response.Redirect(oauthOptions.ErrorPath + $"?message={ctx.Failure?.Message}");
                        ctx.HandleResponse();
                        return Task.CompletedTask;
                    }
                };
            });
        
        builder.Services.AddSingleton<ITicketStore, SessionTicketStore>();
        builder.Services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, CookiePostConfigurator>();
    }
}