using eCinema.Server.Security.Authentication.Ticket;
using eSecurity.Client.BFF.Security.Authentication.OpenIdConnect;
using eSecurity.Client.BFF.Security.Authentication.OpenIdConnect.Discovery;
using eSecurity.Client.BFF.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Client.BFF.Security.Authentication.OpenIdConnect.Token.Validation;
using eSecurity.Client.BFF.Security.Authentication.Session;
using eSecurity.Client.BFF.Security.Authentication.Ticket;
using eSecurity.Client.BFF.Security.Authentication.Token;
using eSecurity.Client.BFF.Security.Cookies;
using eSecurity.Client.BFF.Security.Identity;
using eSystem.Core.Common.Configuration;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Token.Validation;
using eSystem.Core.Security.Identity.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace eSecurity.Client.BFF.Security.Authentication;

public static class AuthenticationExtensions
{
    public static void AddAuthentication(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITokenValidator, LogoutTokenValidator>();
        builder.Services.AddScoped<ITokenHandler, TokenHandler>();
        builder.Services.AddScoped<ISessionManager, SessionManager>();
        builder.Services.AddSingleton<IOpenIdDiscoveryProvider, OpenIdDiscoveryProvider>();
        builder.Services.AddSingleton<IUserIdProvider, SubjectUserIdProvider>();
        builder.Services.AddSingleton<ITicketStore, SessionTicketStore>();

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
                        ctx.Response.Redirect(oauthOptions.ErrorPath + ctx.Failure?.Message);
                        ctx.HandleResponse();
                        return Task.CompletedTask;
                    }
                };
            });
        
        builder.Services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, CookiePostConfigurator>();
    }
}