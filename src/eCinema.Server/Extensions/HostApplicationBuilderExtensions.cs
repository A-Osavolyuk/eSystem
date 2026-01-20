using System.Text.Json.Serialization;
using eCinema.Server.Common.Errors;
using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Configuration;
using eSystem.Core.Common.Documentation;
using eSystem.Core.Common.Error;
using eSystem.Core.Common.Gateway;
using eSystem.Core.Common.Versioning;
using eSystem.Core.Security.Authentication.Oidc.Constants;
using eSystem.Core.Security.Identity.Claims;
using eSystem.ServiceDefaults;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OAuthOptions = eSystem.Core.Security.Authorization.OAuth.OAuthOptions;

namespace eCinema.Server.Extensions;

public static class HostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddServices()
        {
            builder.AddServiceDefaults();
            builder.AddDocumentation();
            builder.AddVersioning();
            builder.AddRedisCache();
            builder.AddExceptionHandling<GlobalExceptionHandler>();
            
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
                    
                    options.SaveTokens = oauthOptions.SaveTokens;
                    options.MapInboundClaims = false;
                    options.CallbackPath = oauthOptions.CallbackPath;
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    
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
            
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
            });
            
            builder.Services.AddOpenApi();
            builder.Services.AddGateway();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddControllers()
                .AddJsonOptions(cfg =>
                {
                    cfg.JsonSerializerOptions.WriteIndented = true;
                    cfg.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });;
        }
    }
}