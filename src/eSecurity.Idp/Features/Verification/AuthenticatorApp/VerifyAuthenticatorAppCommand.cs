using System.Text.Json.Serialization;
using eSecurity.Core.Responses.Verification;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.TwoFactor.AuthenticatorApp;
using eSecurity.Idp.Security.Authentication.TwoFactor.Secret;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Cryptography.Protection.Constants;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Server.Exceptions;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Idp.Features.Verification.AuthenticatorApp;

public sealed record VerifyAuthenticatorAppCommand : IRequest<Result>
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    
    [JsonPropertyName("operation_type")]
    public OperationType OperationType { get; set; }
}

public sealed class VerifyAuthenticatorAppCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    ISecretManager secretManager,
    IDataProtectionProvider protectionProvider,
    IVerificationCommandService verificationCommandService,
    IOptions<VerificationConfiguration> options) : IRequestHandler<VerifyAuthenticatorAppCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly ISecretManager _secretManager = secretManager;
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;
    private readonly VerificationConfiguration _configuration = options.Value;

    public async Task<Result> Handle(VerifyAuthenticatorAppCommand request, 
        CancellationToken cancellationToken = default)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var secret = await _secretManager.GetAsync(user, cancellationToken);
        if (secret is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Authenticator app is not set up for this user."
            });
        }

        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Secret);
        var unprotectedSecret = protector.Unprotect(secret.ProtectedSecret);
        
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new ValidationException("Code is required");
        
        if (!AuthenticatorUtils.VerifyCode(request.Code, unprotectedSecret))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid code."
            });
        }

        var requestEntity = new VerificationRequestEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Operation = request.OperationType,
            Method = VerificationMethod.AuthenticatorApp,
            Status = VerificationStatus.Approved,
            ExpiredAt = DateTimeOffset.UtcNow.Add(_configuration.Timestamp)
        };

        var verificationResult = await _verificationCommandService.CreateAsync(requestEntity, cancellationToken);
        if (!verificationResult.Succeeded) 
            return verificationResult;

        var response = new VerificationResponse { VerificationId = requestEntity.Id };
        return Results.Success(SuccessCodes.Ok, response);
    }
}

public sealed class VerifyAuthenticatorAppCommandValidator : IRequestValidator<VerifyAuthenticatorAppCommand>
{
    public async ValueTask<Result> Validate(VerifyAuthenticatorAppCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'code' is required"
            });
        }

        if (request.OperationType == OperationType.None)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'operation_type' is invalid"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}