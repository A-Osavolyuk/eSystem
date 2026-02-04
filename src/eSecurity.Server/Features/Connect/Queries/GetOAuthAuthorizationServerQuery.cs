using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Authorization.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Authorization.OAuth.Constants;
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
            DeviceAuthorizationEndpoint = "https://localhost:6201/api/v1/connect/device_authorization",
            ResponseTypesSupported = [ResponseTypes.Code],
            GrantTypesSupported =
            [
                GrantTypes.AuthorizationCode,
                GrantTypes.RefreshToken,
                GrantTypes.ClientCredentials
            ],
            PromptValuesSupported =
            [
                PromptTypes.None,
                PromptTypes.Login,
                PromptTypes.Consent,
                PromptTypes.SelectAccount
            ],
            SubjectTypesSupported = [SubjectTypes.Public, SubjectTypes.Pairwise],
            TokenEndpointAuthMethodsSupported =
            [
                TokenAuthMethods.ClientSecretBasic,
                TokenAuthMethods.ClientSecretJwt,
                TokenAuthMethods.ClientSecretPost,
                TokenAuthMethods.PrivateKeyJwt,
                TokenAuthMethods.None
            ],
            CodeChallengeMethodsSupported = [ChallengeMethods.S256],
            ScopesSupported =
            [
                ScopeTypes.OfflineAccess,
                ScopeTypes.Email,
                ScopeTypes.Profile,
                ScopeTypes.Phone,
                ScopeTypes.Address
            ]
        };

        return Task.FromResult(Results.Ok(discovery));
    }
}