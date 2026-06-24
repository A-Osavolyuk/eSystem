using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Passkeys;

public sealed class ChangeSoftwareKeyNameCommand : IRequest<Result>
{
    [JsonPropertyName("passkey_id")]
    public required Guid PasskeyId { get; set; }
    
    [JsonPropertyName("display_name")]
    public required string DisplayName { get; set; }
}

public sealed class ChangeSoftwareKeyNameCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IPasskeyManager passkeyManager) : IRequestHandler<ChangeSoftwareKeyNameCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;

    public async Task<Result> Handle(ChangeSoftwareKeyNameCommand request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        if (!await _passkeyManager.HasAsync(user, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "User does not have any passkeys"
            });
        }

        var passkey = await _passkeyManager.FindByIdAsync(request.PasskeyId, cancellationToken);
        if (passkey is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Passkey not found"
            });
        }

        passkey.DisplayName = request.DisplayName;

        var result = await _passkeyManager.UpdateAsync(passkey, cancellationToken);
        return result;
    }
}

public sealed class ChangeSoftwareKeyNameCommandValidator : IRequestValidator<ChangeSoftwareKeyNameCommand>
{
    public async ValueTask<Result> Validate(ChangeSoftwareKeyNameCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'display_name' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}