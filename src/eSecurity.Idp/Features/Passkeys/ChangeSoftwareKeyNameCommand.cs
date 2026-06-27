using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Passkeys;

public sealed class ChangeSoftwareKeyNameCommand : IRequest<Result>
{
    [JsonPropertyName("passkey_id")]
    public Guid SoftwareKeyId { get; set; }
    
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }
}

public sealed class ChangeSoftwareKeyNameCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    ISoftwareKeyQueryService softwareKeyQueryService,
    ISoftwareKeyCommandService softwareKeyCommandService) : IRequestHandler<ChangeSoftwareKeyNameCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly ISoftwareKeyQueryService _softwareKeyQueryService = softwareKeyQueryService;
    private readonly ISoftwareKeyCommandService _softwareKeyCommandService = softwareKeyCommandService;

    public async Task<Result> Handle(ChangeSoftwareKeyNameCommand request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var softwareKeys = await _softwareKeyQueryService.ListByUserAsync(user.Id, cancellationToken);
        
        if (softwareKeys.Count == 0)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "User does not have any software key"
            });
        }

        var softwareKey = softwareKeys.FirstOrDefault(x => x.Id == request.SoftwareKeyId);
        if (softwareKey is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Passkey not found"
            });
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
            throw new ValidationException("DisplayName is required");

        return await _softwareKeyCommandService.ChangeDisplayNameAsync(
            softwareKey, request.DisplayName, cancellationToken);
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