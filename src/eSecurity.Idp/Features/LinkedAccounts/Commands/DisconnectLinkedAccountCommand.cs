using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Requests;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Security.Authorization.LinkedAccount;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.LinkedAccounts.Commands;

public record DisconnectLinkedAccountCommand(DisconnectLinkedAccountRequest Request) : IRequest<Result>;

public class DisconnectLinkedAccountCommandHandler(
    ILinkedAccountManager linkedAccountManager,
    ICurrentUserAccessor currentUserAccessor,
    IVerificationQueryService verificationQueryService,
    IVerificationCommandService verificationCommandService) : IRequestHandler<DisconnectLinkedAccountCommand, Result>
{
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IVerificationQueryService _verificationQueryService = verificationQueryService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;

    public async Task<Result> Handle(DisconnectLinkedAccountCommand request, CancellationToken cancellationToken)
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

        var verification = await _verificationQueryService.GetByIdAsync(user.Id, 
            request.Request.VerificationId, cancellationToken);
        
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

        var linkedAccount = await _linkedAccountManager.GetAsync(user, request.Request.Type, cancellationToken);
        if (linkedAccount is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Linked account not found."
            });
        }

        var result = await _linkedAccountManager.RemoveAsync(linkedAccount, cancellationToken);
        return result;
    }
}