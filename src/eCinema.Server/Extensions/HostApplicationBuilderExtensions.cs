using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using eCinema.Server.Common.Errors;
using eCinema.Server.Security.Cors;
using eSystem.Core.Common.Configuration;
using eSystem.Core.Common.Documentation;
using eSystem.Core.Common.Error;
using eSystem.Core.Common.Gateway;
using eSystem.Core.Http.Constants;
using eSystem.Core.Security.Authentication.Oidc.Constants;
using eSystem.Core.Security.Identity.Claims;
using eSystem.ServiceDefaults;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

namespace eCinema.Server.Extensions;

public static class HostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddServices()
        {
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<TokenHandler>();
            
            builder.AddServiceDefaults();
            builder.AddDocumentation();
            builder.AddExceptionHandling<GlobalExceptionHandler>();
            builder.AddProxy();
            builder.AddAuthentication();

            builder.Services.AddOpenApi();
            builder.Services.AddGateway();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicies.SpaOnly, policy =>
                {
                    policy.WithOrigins("https://localhost:6511");
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowCredentials();
                });
                
                options.AddPolicy(CorsPolicies.ExternalOnly, policy =>
                {
                    policy.AllowAnyOrigin();
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                });
            });

            builder.Services.AddControllers()
                .AddJsonOptions(cfg =>
                {
                    cfg.JsonSerializerOptions.WriteIndented = true;
                    cfg.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });
        }

        private void AddProxy()
        {
            builder.Services.AddReverseProxy()
                .LoadFromMemory(
                    [
                        new RouteConfig()
                        {
                            RouteId = "proxy-route", ClusterId = "proxy-cluster",
                            Match = new RouteMatch { Path = "/api/v1/{**catch-all}" }
                        }
                    ],
                    [
                        new ClusterConfig()
                        {
                            ClusterId = "proxy-cluster",
                            Destinations = new Dictionary<string, DestinationConfig>()
                            {
                                ["security-destination"] = new()
                                {
                                    Address = builder.Configuration["PROXY_HTTPS"]!
                                }
                            }
                        }
                    ])
                .AddTransforms(context =>
                {
                    context.AddRequestTransform(async request =>
                    {
                        var httpContent = request.HttpContext;
                        if (httpContent.User.Identity?.IsAuthenticated == true)
                        {
                            var tokenHandler = httpContent.RequestServices.GetRequiredService<TokenHandler>();
                            var accessToken = await tokenHandler.GetTokenAsync();
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                var header = new AuthenticationHeaderValue(AuthenticationTypes.Bearer, accessToken);
                                request.ProxyRequest.Headers.Authorization = header;
                            }
                        }
                    });
                });
        }

        private void AddAuthentication()
        {
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
        }
    }
}