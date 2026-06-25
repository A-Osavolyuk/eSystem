using System.Text.Json.Serialization;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Security.Authorization.LinkedAccount;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Server.Exceptions;

namespace eSecurity.Idp.Features.Email;

public sealed class RemoveEmailCommand : IRequest<Result>
{
    [JsonPropertyName("verification_id")]
    public Guid VerificationId { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}

public sealed class RemoveEmailCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    ISoftwareKeyManager softwareKeyManager,
    ILinkedAccountManager linkedAccountManager,
    IEmailQueryService emailQueryService,
    IEmailCommandService emailCommandService,
    IVerificationQueryService verificationQueryService,
    IVerificationCommandService verificationCommandService) : IRequestHandler<RemoveEmailCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly ISoftwareKeyManager _softwareKeyManager = softwareKeyManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IEmailCommandService _emailCommandService = emailCommandService;
    private readonly IVerificationQueryService _verificationQueryService = verificationQueryService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;

    public async Task<Result> Handle(RemoveEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email is require");
        
        var email = await _emailQueryService.GetByEmailAsync(user.Id, request.Email, cancellationToken);
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
            var passkeys = await _softwareKeyManager.GetAllAsync(user, cancellationToken);
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

        var result = await _emailCommandService.RemoveAsync(user.Id, request.Email, cancellationToken);
        return result;
    }
}

public sealed class RemoveEmailCommandValidator : IRequestValidator<RemoveEmailCommand>
{
    public async ValueTask<Result> Validate(RemoveEmailCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'email' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}