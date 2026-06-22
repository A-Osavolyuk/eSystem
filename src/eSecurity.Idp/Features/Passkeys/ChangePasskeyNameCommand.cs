using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Passkeys;

public record ChangePasskeyNameCommand : IRequest<Result>
{
    [JsonPropertyName("passkey_id")]
    public required Guid PasskeyId { get; set; }
    
    [JsonPropertyName("display_name")]
    public required string DisplayName { get; set; }
}

public class ChangePasskeyNameCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IPasskeyManager passkeyManager) : IRequestHandler<ChangePasskeyNameCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;

    public async Task<Result> Handle(ChangePasskeyNameCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        if (!userResult.TryGetValue(out var user))
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
            });
        }

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