using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Discovery;

namespace eSecurity.Server.Features.Connect.Queries;

public sealed record GetOAuthAuthorizationServerQuery : IRequest<Result>;

public sealed class GetOAuthAuthorizationServerQueryHandler : IRequestHandler<GetOAuthAuthorizationServerQuery, Result>
{
    public Task<Result> Handle(GetOAuthAuthorizationServerQuery request, CancellationToken cancellationToken)
    {
        var discovery = new AuthorizationServerDiscovery()
        {
            Issuer = "https://localhost:6201",
            AuthorizationEndpoint = "https://localhost:6501/connect/authorize",
            TokenEndpoint = "https://localhost:6201/api/v1/connect/token",
            IntrospectionEndpoint = "https://localhost:6201/api/v1/connect/introspection",
            RevocationEndpoint = "https://localhost:6201/api/v1/connect/revocation",
            JwksUri = "https://localhost:6201/api/v1/connect/.well-known/jwks.json",
            DeviceAuthorizationEndpoint = "https://localhost:6201/api/v1/connect/device-authorization",
            ResponseTypesSupported = [ResponseType.Code],
            GrantTypesSupported =
            [
                GrantType.AuthorizationCode,
                GrantType.RefreshToken,
                GrantType.ClientCredentials,
                GrantType.DeviceCode,
                GrantType.TokenExchange
            ],
            PromptValuesSupported =
            [
                PromptType.None,
                PromptType.Login,
                PromptType.Consent,
                PromptType.SelectAccount
            ],
            SubjectTypesSupported = [SubjectType.Public, SubjectType.Pairwise],
            TokenEndpointAuthMethodsSupported =
            [
                TokenAuthMethod.ClientSecretBasic,
                TokenAuthMethod.ClientSecretJwt,
                TokenAuthMethod.ClientSecretPost,
                TokenAuthMethod.PrivateKeyJwt,
                TokenAuthMethod.None
            ],
            CodeChallengeMethodsSupported = [ChallengeMethod.S256],
            ScopesSupported =
            [
                ScopeTypes.OfflineAccess,
                ScopeTypes.Email,
                ScopeTypes.Profile,
                ScopeTypes.Phone,
                ScopeTypes.Address,
                ScopeTypes.ClientRegistration,
                ScopeTypes.Transformation,
                ScopeTypes.Delegation
            ]
        };

        return Task.FromResult(Results.Success(SuccessCodes.Ok, discovery));
    }
}