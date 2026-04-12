using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.Claims;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Token;
using eSystem.Core.Security.Authorization.OAuth.Token.ClientCredentials;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.ClientCredentials;

public sealed class ClientCredentialsStrategy(
    IClientManager clientManager,
    IClaimFactoryProvider claimFactoryProvider,
    ITokenBuilderProvider tokenBuilderProvider,
    IHasherProvider hasherProvider,
    ITokenManager tokenManager,
    ITokenFactoryProvider tokenFactoryProvider,
    IOptions<TokenConfigurations> options) : ITokenStrategy
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;
    private readonly IHasherProvider _hasherProvider = hasherProvider;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
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

        var client = await _clientManager.FindByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error
            {
                Code = ErrorCode.InvalidClient,
                Description = "Client was not found."
            });
        }

        if (!client.HasGrantType(request.GrantType))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.UnsupportedGrantType,
                Description = $"'{request.GrantType}' grant is not supported by client."
            });
        }

        var grantedScopes = client.AllowedScopes.Select(x => x.Scope.Value);
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

        var factoryOptions = new TokenFactoryOptions() { AllowedScopes = allowedScopes };
        var accessTokenFactory = _tokenFactoryProvider.GetFactory(TokenType.AccessToken);
        var accessTokenResult = await accessTokenFactory.CreateAsync(client, 
            factoryOptions: factoryOptions, cancellationToken: cancellationToken);
        
        if (!accessTokenResult.Succeeded)
        {
            var error = accessTokenResult.GetError();
            return Results.ServerError(ServerErrorCode.InternalServerError, error);
        }

        if (!accessTokenResult.TryGetValue(out var accessToken))
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }
            
        response.AccessToken = accessToken;

        return Results.Success(SuccessCodes.Ok, response);
    }
}