using eSecurity.Idp.Common.Binding.Binders;
using eSystem.Core.Server.Binding;
using eSystem.Core.Server.Security.Authorization.OAuth.Token;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.AuthorizationCode;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.Ciba;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.ClientCredentials;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.DeviceCode;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.RefreshToken;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.TokenExchange;

namespace eSecurity.Idp.Common.Binding;

public static class BindingExtensions
{
    public static void AddDataBinding(this IServiceCollection services)
    {
        services.AddSingleton<IFormBindingProvider, FormBindingProvider>();
        services.AddTransient<IFormBinder<TokenRequest>, TokenRequestBinder>();
        services.AddTransient<IFormBinder<AuthorizationCodeRequest>, AuthorizationCodeRequestBinder>();
        services.AddTransient<IFormBinder<RefreshTokenRequest>, RefreshTokenRequestBinder>();
        services.AddTransient<IFormBinder<ClientCredentialsRequest>, ClientCredentialsRequestBinder>();
        services.AddTransient<IFormBinder<DeviceCodeRequest>, DeviceCodeRequestBinder>();
        services.AddTransient<IFormBinder<TokenExchangeRequest>, TokenExchangeRequestBinder>();
        services.AddTransient<IFormBinder<CibaRequest>, CibaRequestBinder>();
    }
}