using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Cryptography.Tokens;
using eSecurity.Idp.Security.Cryptography.Tokens.Access;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Server.Security.Authorization.OAuth.Token;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.ClientCredentials;

namespace eSecurity.Idp.Security.Authorization.Token.ClientCredentials;

public sealed class ClientCredentialsStrategy(
    ITokenFactoryProvider tokenFactoryProvider,
    IClientQueryService clientQueryService,
    IOptions<TokenConfigurations> options) : ITokenStrategy
{
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;

    public async ValueTask<Result> ExecuteAsync(TokenRequest tokenRequest,
        CancellationToken cancellationToken = default)
    {
        if (tokenRequest is not ClientCredentialsRequest request)
            throw new NotSupportedException("Payload type must be 'ClientCredentialsRequest'");

        if (string.IsNullOrEmpty(request.ClientSecret))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "client_secret is required"
            });
        }

        if (string.IsNullOrEmpty(request.Scope))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "scope is required"
            });
        }

        var client = await _clientQueryService.GetByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error
            {
                Code = ErrorCode.InvalidClient,
                Description = "Client was not found."
            });
        }

        var clientGrantTypes = await _clientQueryService.GetSupportedGrantTypesAsync(client.Id, cancellationToken);
        if (clientGrantTypes.All(x => x.Grant.Grant != request.GrantType))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.UnsupportedGrantType,
                Description = $"'{request.GrantType}' grant is not supported by client."
            });
        }

        var clientScopes = await _clientQueryService.GetAllowedScopesAsync(client.Id, cancellationToken);
        var grantedScopes = clientScopes.Select(x => x.Scope.Value);
        var requestScopes = request.Scope!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var allowedScopes = grantedScopes.Intersect(requestScopes).ToList();

        if (allowedScopes.Count == 0)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidScope,
                Description = "None of the requested scopes are allowed for this client."
            });
        }

        var response = new ClientCredentialsResponse
        {
            ExpiresIn = (int)_tokenConfigurations.DefaultAccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenType.Bearer
        };

        var factoryContext = new AccessTokenFactoryContext { Client = client };
        var factoryOptions = new TokenFactoryOptions { AllowedScopes = allowedScopes };
        var accessTokenFactory = _tokenFactoryProvider.GetFactory<AccessTokenFactoryContext>();
        var accessTokenResult = await accessTokenFactory.CreateAsync(factoryContext, factoryOptions, cancellationToken);
        
        if (!accessTokenResult.Succeeded)
        {
            var error = accessTokenResult.GetError();
            return Results.ServerError(ServerErrorCode.InternalServerError, error);
        }

        if (!accessTokenResult.TryGetValue(out var accessToken))
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }
            
        response.AccessToken = accessToken;

        return Results.Success(SuccessCodes.Ok, response);
    }
}