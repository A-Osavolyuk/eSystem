using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Authorization.Constants;
using eSecurity.Server.Security.Cryptography.Keys;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;
using eSystem.Core.Security.Authentication.OpenIdConnect.Registration;

namespace eSecurity.Server.Features.Connect.Commands;

public sealed record CreateClientCommand(RegistrationRequest Request) : IRequest<Result>;

public sealed class CreateClientCommandHandler(
    IClientManager clientManager,
    IKeyFactory keyFactory,
    IOptions<OpenIdConfiguration> options) : IRequestHandler<CreateClientCommand, Result>
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly IKeyFactory _keyFactory = keyFactory;
    private readonly OpenIdConfiguration _options = options.Value;

    public async Task<Result> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var client = new ClientEntity()
        {
            Id = Guid.CreateVersion7(),
            Name = request.Request.ClientName,
            AccessTokenType = AccessTokenType.Jwt,
            ClientUri = request.Request.ClientUri,
            LogoUri = request.Request.LogoUri,
            Secret = _keyFactory.Create(32),
            RefreshTokenRotationEnabled = false,
            RefreshTokenLifetime = TimeSpan.FromDays(30)
        };

        var invalidResponseTypes = request.Request.ResponseTypes
            .Except(_options.ResponseTypesSupported)
            .ToList();

        if (invalidResponseTypes.Count > 0)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = $"'{string.Join(',', invalidResponseTypes)}' are not valid response types."
            });
        }

        client.ResponseTypes = request.Request.ResponseTypes.Select(type => new ClientResponseTypeEntity()
        {
            Id = Guid.CreateVersion7(),
            ClientId = client.Id,
            ResponseType = type
        }).ToList();

        var scopes = request.Request.Scope.Split(' ');
        var invalidScopes = scopes.Except(_options.ScopesSupported).ToList();
        if (invalidScopes.Count > 0)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = $"'{string.Join(',', invalidScopes)}' are invalid scope values."
            });
        }

        client.AllowOfflineAccess = scopes.Contains(ScopeTypes.OfflineAccess);

        var invalidGrants = request.Request.GrantTypes.Except(_options.GrantTypesSupported).ToList();
        if (invalidGrants.Count > 0)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = $"'{string.Join(',', invalidGrants)}' are invalid scope values."
            });
        }

        client.GrantTypes = request.Request.GrantTypes.Select(grant => new ClientGrantTypeEntity()
        {
            Id = Guid.CreateVersion7(),
            ClientId = client.Id,
            Type = grant,
        }).ToList();

        var invalidTokenAuthMethods = request.Request.TokenEndpointAuthMethods
            .Except(_options.TokenEndpointAuthMethodsSupported)
            .ToList();

        if (invalidTokenAuthMethods.Count > 0)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = $"'{string.Join(',', invalidTokenAuthMethods)}' are invalid token endpoint auth methods."
            });
        }

        client.TokenAuthMethods = request.Request.TokenEndpointAuthMethods.Select(method =>
            new ClientTokenAuthMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = client.Id,
                Method = method
            }).ToList();

        client.ClientType = request.Request.TokenEndpointAuthMethods.Contains(TokenAuthMethods.None)
            ? ClientType.Public
            : ClientType.Confidential;

        client.RequirePkce = request.Request.RequirePkce ?? client.ClientType == ClientType.Public;
        client.RequireClientSecret = client.ClientType == ClientType.Confidential;

        if (request.Request.RedirectUris.Length == 0)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "Redirect URIs are mandatory."
            });
        }

        client.Uris = request.Request.RedirectUris.Select(uri => new ClientUriEntity()
        {
            Id = Guid.CreateVersion7(),
            Type = UriType.Redirect,
            ClientId = client.Id,
            Uri = uri
        }).ToList();

        if (request.Request.PostLogoutRedirectUris is not null)
        {
            foreach (var uri in request.Request.PostLogoutRedirectUris)
            {
                client.Uris.Add(new ClientUriEntity()
                {
                    Id = Guid.CreateVersion7(),
                    Type = UriType.PostLogoutRedirect,
                    ClientId = client.Id,
                    Uri = uri
                });
            }
        }

        if (request.Request.BackchannelLogoutSessionSupported.HasValue &&
            request.Request.BackchannelLogoutSessionSupported.Value &&
            !string.IsNullOrEmpty(request.Request.BackchannelLogoutUri))
        {
            client.AllowBackChannelLogout = true;
            client.Uris.Add(new ClientUriEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = UriType.BackChannelLogout,
                ClientId = client.Id,
                Uri = request.Request.BackchannelLogoutUri
            });
        }

        if (request.Request.FrontchannelLogoutSessionSupported.HasValue &&
            request.Request.FrontchannelLogoutSessionSupported.Value &&
            !string.IsNullOrEmpty(request.Request.FrontchannelLogoutUri))
        {
            client.AllowFrontChannelLogout = true;
            client.Uris.Add(new ClientUriEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = UriType.FrontChannelLogout,
                ClientId = client.Id,
                Uri = request.Request.FrontchannelLogoutUri
            });
        }

        SubjectType? subjectType = request.Request.SubjectType switch
        {
            SubjectTypes.Public => SubjectType.Public,
            SubjectTypes.Pairwise => SubjectType.Pairwise,
            _ => null
        };

        if (subjectType is null)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = $"'{request.Request.SubjectType}' is not a valid subject type."
            });
        }

        if (subjectType == SubjectType.Pairwise)
        {
            if (string.IsNullOrEmpty(request.Request.SectorIdentifierUri))
            {
                return Results.BadRequest(new Error
                {
                    Code = ErrorTypes.OAuth.InvalidRequest,
                    Description = "'sector_identifier_uri' is mandatory for pairwise subject type."
                });
            }

            client.SectorIdentifierUri = request.Request.SectorIdentifierUri;
        }

        client.SubjectType = subjectType.Value;

        var result = await _clientManager.CreateAsync(client, cancellationToken);
        if (!result.Succeeded) return result;

        var response = new RegistrationResponse()
        {
            //TODO: Implement registration access token generation
            RegistrationAccessToken = "",
            RegistrationClientUri = "",
            ClientId = client.Id.ToString(),
            ClientIdIssuedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ClientSecret = client.Secret,
            ClientSecretExpiresAt = 0,
            ClientName = client.Name,
            InternalClientType = client.ClientType.ToString().ToLower(),
            Scope = request.Request.Scope,
            RedirectUris = request.Request.RedirectUris,
            GrantTypes = request.Request.GrantTypes,
            ResponseTypes = request.Request.ResponseTypes,
            PostLogoutRedirectUris = request.Request.PostLogoutRedirectUris,
            TokenEndpointAuthMethods = request.Request.TokenEndpointAuthMethods,
            BackchannelLogoutUri = request.Request.BackchannelLogoutUri,
            BackchannelLogoutSessionSupported = request.Request.BackchannelLogoutSessionSupported,
            FrontchannelLogoutUri = request.Request.FrontchannelLogoutUri,
            FrontchannelLogoutSessionSupported = request.Request.FrontchannelLogoutSessionSupported,
            RequestUris = request.Request.RequestUris,
            SubjectType = request.Request.SubjectType,
            ClientUri = request.Request.ClientUri,
            LogoUri = request.Request.LogoUri,
            RequirePkce = request.Request.RequirePkce,
            SectorIdentifierUri = request.Request.SectorIdentifierUri,
            Contacts = request.Request.Contacts,
            DefaultAcrValues = request.Request.DefaultAcrValues,
            DefaultMaxAge = request.Request.DefaultMaxAge,
            JwksUri = request.Request.JwksUri,
            PolicyUri = request.Request.PolicyUri,
            RequireAuthTime = request.Request.RequireAuthTime,
            TosUri = request.Request.TosUri,
        };

        return Results.Created(response);
    }
}