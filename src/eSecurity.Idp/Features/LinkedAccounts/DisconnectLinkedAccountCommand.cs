using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Idp.Security.Authorization.LinkedAccount;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.LinkedAccounts;

public record DisconnectLinkedAccountCommand : IRequest<Result>
{
    [JsonPropertyName("verification_id")]
    public Guid VerificationId { get; set; }
    
    [JsonPropertyName("type")]
    public LinkedAccountType Type { get; set; }
}

public class DisconnectLinkedAccountCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    ILinkedAccountQueryService linkedAccountQueryService,
    ILinkedAccountCommandService linkedAccountCommandService,
    IVerificationQueryService verificationQueryService,
    IVerificationCommandService verificationCommandService) : IRequestHandler<DisconnectLinkedAccountCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly ILinkedAccountQueryService _linkedAccountQueryService = linkedAccountQueryService;
    private readonly ILinkedAccountCommandService _linkedAccountCommandService = linkedAccountCommandService;
    private readonly IVerificationQueryService _verificationQueryService = verificationQueryService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;

    public async Task<Result> Handle(DisconnectLinkedAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var verification = await _verificationQueryService.GetByIdAsync(user.Id, 
            request.VerificationId, cancellationToken);
        
        if (verification is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid verification request"
            });
        }

        var verificationResult = await _verificationCommandService.ConsumeAsync(verification, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var linkedAccount = await _linkedAccountQueryService.GetByTypeAsync(user.Id, request.Type, cancellationToken);
        if (linkedAccount is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Linked account not found."
            });
        }

        var result = await _linkedAccountCommandService.RemoveAsync(linkedAccount.Id, cancellationToken);
        return result;
    }
}

public sealed class DisconnectLinkedAccountCommandValidator : IRequestValidator<DisconnectLinkedAccountCommand>
{
    public async ValueTask<Result> Validate(DisconnectLinkedAccountCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Type == LinkedAccountType.None)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'type' is invalid"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}