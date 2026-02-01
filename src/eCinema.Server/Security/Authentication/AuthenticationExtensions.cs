using eCinema.Server.Security.Authentication.OpenIdConnect.Discovery;
using eCinema.Server.Security.Authentication.OpenIdConnect.Token;
using eCinema.Server.Security.Authentication.OpenIdConnect.Token.Validation;
using eCinema.Server.Security.Authentication.Ticket;
using eCinema.Server.Security.Identity;
using eSystem.Core.Common.Configuration;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Authentication.OpenIdConnect.Token.Validation;
using eSystem.Core.Security.Identity.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace eCinema.Server.Security.Authentication;

public static class AuthenticationExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddAuthentication()
        {
            builder.Services.AddScoped<ITokenHandler, TokenHandler>();
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
                    options.Cookie.Name = "eCinema.Authentication.State";
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
                    options.ResponseType = ResponseTypes.Code;
                    options.UsePkce = true;

                    options.MapInboundClaims = false;
                    options.GetClaimsFromUserInfoEndpoint = true;
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

                    options.Prompt = "login consent";

                    options.Scope.Clear();
                    options.Scope.Add(Scopes.OpenId);
                    options.Scope.Add(Scopes.OfflineAccess);
                    options.Scope.Add(Scopes.Email);
                    options.Scope.Add(Scopes.Phone);
                    options.Scope.Add(Scopes.Profile);
                    options.Scope.Add(Scopes.Address);

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

            builder.Services.AddSingleton<ITicketStore, SessionTicketStore>();
            builder.Services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, CookiePostConfigurator>();
        }
    }
}