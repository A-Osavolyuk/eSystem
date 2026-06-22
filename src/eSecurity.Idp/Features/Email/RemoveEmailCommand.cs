using System.Text.Json.Serialization;
using eSecurity.Core.Requests;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Security.Authorization.LinkedAccount;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Email;

public record RemoveEmailCommand(RemoveEmailRequest Request) : IRequest<Result>
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; set; }
    
    [JsonPropertyName("email")]
    public required string Email { get; set; }
}

public class RemoveEmailCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IPasskeyManager passkeyManager,
    ILinkedAccountManager linkedAccountManager,
    IEmailQueryService emailQueryService,
    IEmailCommandService emailCommandService,
    IVerificationQueryService verificationQueryService,
    IVerificationCommandService verificationCommandService) : IRequestHandler<RemoveEmailCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IEmailCommandService _emailCommandService = emailCommandService;
    private readonly IVerificationQueryService _verificationQueryService = verificationQueryService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;

    public async Task<Result> Handle(RemoveEmailCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        var user = userResult.GetValue();
        var email = await _emailQueryService.GetByEmailAsync(user.Id, request.Request.Email, cancellationToken);
        if (email is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.InvalidEmail,
                Description = "User doesn't owe this email."
            });
        }

        if (email.Type == EmailType.Primary)
        {
            var passkeys = await _passkeyManager.GetAllAsync(user, cancellationToken);
            if (passkeys.Count == 0)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidEmail,
                    Description = "Cannot remove the primary email, because it is the only authentication method"
                });
            }

            if (await _linkedAccountManager.HasAsync(user, cancellationToken))
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.LinkedAccountConnected,
                    Description = "Cannot remove the primary email, because there are one or more linked accounts"
                });
            }
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

        var result = await _emailCommandService.RemoveAsync(user.Id, request.Request.Email, cancellationToken);
        return result;
    }
}