using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Authentication.TwoFactor.Secret;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Cryptography.Protection.Constants;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Server.Exceptions;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Idp.Features.TwoFactor;

public sealed class ReconfigureAuthenticatorCommand : IRequest<Result>
{
    [JsonPropertyName("verification_id")]
    public Guid VerificationId { get; set; }
    
    [JsonPropertyName("secret")]
    public string? Secret { get; set; }
}

public sealed class ReconfigureAuthenticatorCommandHandler(
    ISecretManager secretManager,
    IVerificationQueryService verificationQueryService,
    IVerificationCommandService verificationCommandService,
    ICurrentUserAccessor currentUserAccessor,
    IDataProtectionProvider protectionProvider) : IRequestHandler<ReconfigureAuthenticatorCommand, Result>
{
    private readonly ISecretManager _secretManager = secretManager;
    private readonly IVerificationQueryService _verificationQueryService = verificationQueryService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;

    public async Task<Result> Handle(ReconfigureAuthenticatorCommand request, CancellationToken cancellationToken)
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
        if (!verificationResult.Succeeded) 
            return verificationResult;

        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Secret);
        if (string.IsNullOrWhiteSpace(request.Secret))
            throw new ValidationException("Secret is required");
        
        var protectedSecret = protector.Protect(request.Secret);
        var userSecret = await _secretManager.GetAsync(user, cancellationToken);
        if (userSecret is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Secret not found"
            });
        }

        userSecret.ProtectedSecret = protectedSecret;

        var result = await _secretManager.UpdateAsync(userSecret, cancellationToken);
        return result;
    }
}

public sealed class ReconfigureAuthenticatorCommandValidator : IRequestValidator<ReconfigureAuthenticatorCommand>
{
    public async ValueTask<Result> Validate(ReconfigureAuthenticatorCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Secret))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'secret' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}