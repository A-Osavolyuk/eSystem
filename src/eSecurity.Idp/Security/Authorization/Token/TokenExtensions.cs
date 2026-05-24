using eSecurity.Idp.Security.Authorization.Constants;
using eSecurity.Idp.Security.Authorization.Token.AuthorizationCode;
using eSecurity.Idp.Security.Authorization.Token.ClientCredentials;
using eSecurity.Idp.Security.Authorization.Token.DeviceCode;
using eSecurity.Idp.Security.Authorization.Token.RefreshToken;
using eSecurity.Idp.Security.Authorization.Token.TokenExchange;
using eSecurity.Idp.Security.Authorization.Token.TokenExchange.Delegation;
using eSecurity.Idp.Security.Authorization.Token.TokenExchange.Transformation;
using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Idp.Security.Authorization.Token;

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
        services.AddKeyedScoped<ITokenExchangeFlow, DelegationTokenExchangeFlow>(TokenExchangeFlow.Delegation);
        services.AddScoped<ITokenTransformationHandlerResolver, TokenTransformationHandlerResolver>();
        services.AddKeyedScoped<ITokenTransformationHandler, JwtTokenTransformationHandler>(TokenKind.Jwt);
        services.AddKeyedScoped<ITokenTransformationHandler, OpaqueTokenTransformationHandler>(TokenKind.Opaque);
        services.AddScoped<ITokenDelegationHandlerResolver, TokenDelegationHandlerResolver>();
        services.AddKeyedScoped<ITokenDelegationHandler, JwtTokenDelegationHandler>(TokenKind.Jwt);
        services.AddKeyedScoped<ITokenDelegationHandler, OpaqueTokenDelegationHandler>(TokenKind.Opaque);
        services.AddScoped<ITokenClaimsExtractor, JwtTokenClaimsExtractor>();
            
        services.AddScoped<ITokenManager, TokenManager>();
        services.AddScoped<ITokenStrategyResolver, TokenStrategyResolver>();
        services.AddKeyedScoped<ITokenStrategy, AuthorizationCodeStrategy>(GrantType.AuthorizationCode);
        services.AddKeyedScoped<ITokenStrategy, RefreshTokenStrategy>(GrantType.RefreshToken);
        services.AddKeyedScoped<ITokenStrategy, ClientCredentialsStrategy>(GrantType.ClientCredentials);
        services.AddKeyedScoped<ITokenStrategy, TokenExchangeStrategy>(GrantType.TokenExchange);
        services.AddKeyedScoped<ITokenStrategy, DeviceCodeStrategy>(GrantType.DeviceCode);
    }
}