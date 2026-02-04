using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authorization.Token.DeviceCode;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Keys;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;
using eSystem.Core.Security.Authorization.OAuth.DeviceAuthorization;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Server.Features.Connect.Commands;

public sealed record DeviceAuthorizationCommand(DeviceAuthorizationRequest Request) : IRequest<Result>;

public sealed class DeviceAuthorizationCommandHandler(
    IClientManager clientManager,
    IHasherProvider hasherProvider,
    IKeyFactory keyFactory,
    IDeviceCodeManager deviceCodeManager,
    IOptions<OpenIdConfiguration> openIdConfiguration,
    IOptions<DeviceAuthorizationOptions> deviceAuthorizationOptions) 
    : IRequestHandler<DeviceAuthorizationCommand, Result>
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly IKeyFactory _keyFactory = keyFactory;
    private readonly IDeviceCodeManager _deviceCodeManager = deviceCodeManager;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Sha512);
    private readonly OpenIdConfiguration _options = openIdConfiguration.Value;
    private readonly DeviceAuthorizationOptions _deviceAuthorizationOptions = deviceAuthorizationOptions.Value;

    public async Task<Result> Handle(DeviceAuthorizationCommand request, CancellationToken cancellationToken)
    {
        var client = await _clientManager.FindByIdAsync(request.Request.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.UnauthorizedClient,
                Description = "Client is not registered to for device authorization flow"
            });
        }

        var scopes = request.Request.Scope.Split(' ');
        var invalidScopes = scopes.Except(_options.ScopesSupported).ToList();
        if (invalidScopes.Count > 0)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidScope,
                Description = $"Scopes are not supported: {string.Join(", ", invalidScopes)}."
            });
        }
        
        var allowedScopes = client.AllowedScopes.Select(x => x.Scope.Value).ToList();
        var unallowedScopes = scopes.Except(allowedScopes).ToList();
        if (unallowedScopes.Count > 0)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidScope,
                Description = $"Scopes are not allowed for this client: {string.Join(", ", unallowedScopes)}."
            });
        }
        
        var deviceCode = _keyFactory.Create(_deviceAuthorizationOptions.DeviceCodeLenght);
        var userCode = _keyFactory.Create(_deviceAuthorizationOptions.UserCodeLenght);
        var formattedUserCode = $"{userCode[..4]}-{userCode[4..]}";
        var deviceCodeEntity = new DeviceCodeEntity
        {
            Id = Guid.CreateVersion7(),
            ClientId = client.Id,
            DeviceCodeHash = _hasher.Hash(deviceCode),
            UserCode = userCode,
            AcrValues = request.Request.AcrValues,
            Scope = request.Request.Scope,
            Interval = _deviceAuthorizationOptions.Interval,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.Add(_deviceAuthorizationOptions.Timestamp),
        };

        var result = await _deviceCodeManager.CreateAsync(deviceCodeEntity, cancellationToken);
        if (!result.Succeeded) return result;

        
        var verificationUriComplete = QueryBuilder.Create()
            .WithUri(_deviceAuthorizationOptions.VerificationUri)
            .WithQueryParam("user_code", formattedUserCode)
            .Build();
        
        var response = new DeviceAuthorizationResponse
        {
            DeviceCode = deviceCode,
            UserCode = formattedUserCode,
            Interval = _deviceAuthorizationOptions.Interval,
            ExpiresIn = (int)_deviceAuthorizationOptions.Timestamp.TotalSeconds,
            VerificationUri = _deviceAuthorizationOptions.VerificationUri,
            VerificationUriComplete = verificationUriComplete
        };
        
        return Results.Ok(response);
    }
}