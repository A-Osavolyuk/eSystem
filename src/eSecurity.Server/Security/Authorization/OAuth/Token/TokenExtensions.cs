using eSecurity.Server.Security.Authorization.Constants;
using eSecurity.Server.Security.Authorization.OAuth.Protocol;
using eSecurity.Server.Security.Authorization.OAuth.Token.AuthorizationCode;
using eSecurity.Server.Security.Authorization.OAuth.Token.ClientCredentials;
using eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;
using eSecurity.Server.Security.Authorization.OAuth.Token.RefreshToken;
using eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange;
using eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Actor;
using eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Claims;
using eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Transformation;
using eSystem.Core.Security.Authorization.OAuth.Constants;

namespace eSecurity.Server.Security.Authorization.OAuth.Token;

public static class TokenExtensions
{
    public static void AddTokenFlow(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationCodeManager, AuthorizationCodeManager>();
        services.AddScoped<IAuthorizationCodeFlowResolver, AuthorizationCodeFlowResolver>();
        services.AddKeyedScoped<IAuthorizationCodeFlow, OidcAuthorizationCodeFlow>(AuthorizationProtocol.OpenIdConnect);
        services.AddKeyedScoped<IAuthorizationCodeFlow, OAuthAuthorizationCodeFlow>(AuthorizationProtocol.OAuth);
        
        services.AddScoped<IRefreshTokenFlowResolver, RefreshTokenFlowResolver>();
        services.AddKeyedScoped<IRefreshTokenFlow, OidcRefreshTokenFlow>(AuthorizationProtocol.OpenIdConnect);
        services.AddKeyedScoped<IRefreshTokenFlow, OAuthRefreshTokenFlow>(AuthorizationProtocol.OAuth);
        
        services.AddScoped<IDeviceCodeFlowResolver, DeviceCodeFlowResolver>();
        services.AddKeyedScoped<IDeviceCodeFlow, OidcDeviceCodeFlow>(AuthorizationProtocol.OpenIdConnect);
        services.AddKeyedScoped<IDeviceCodeFlow, OAuthDeviceCodeFlow>(AuthorizationProtocol.OAuth);
        
        services.AddScoped<ITokenExchangeFlowResolver, TokenExchangeFlowResolver>();
        services.AddKeyedScoped<ITokenExchangeFlow, TransformationTokenExchangeFlow>(TokenExchangeFlow.Transformation);
        services.AddScoped<ITokenTransformationHandlerResolver, TokenTransformationHandlerResolver>();
        services.AddKeyedScoped<ITokenTransformationHandler, JwtTokenTransformationHandler>(TokenKind.Jwt);
        services.AddKeyedScoped<ITokenTransformationHandler, OpaqueTokenTransformationHandler>(TokenKind.Opaque);
        services.AddScoped<ITokenClaimsExtractor, JwtTokenClaimsExtractor>();
        services.AddScoped<ITokenActorExtractor, JwtTokenActorExtractor>();
            
        services.AddScoped<ITokenManager, TokenManager>();
        services.AddScoped<ITokenStrategyResolver, TokenStrategyResolver>();
        services.AddKeyedScoped<ITokenStrategy, AuthorizationCodeStrategy>(GrantTypes.AuthorizationCode);
        services.AddKeyedScoped<ITokenStrategy, RefreshTokenStrategy>(GrantTypes.RefreshToken);
        services.AddKeyedScoped<ITokenStrategy, ClientCredentialsStrategy>(GrantTypes.ClientCredentials);
        services.AddScoped<ITokenRequestMapper, TokenRequestMapper>();
    }
}