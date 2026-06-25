using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Server.Exceptions;

namespace eSecurity.Idp.Features.Email;

public sealed class VerifyEmailCommand : IRequest<Result>
{
    [JsonPropertyName("verification_id")]
    public Guid VerificationId { get; set; }
    
    [JsonPropertyName("email")] 
    public string? Email { get; set; }
    
    [JsonPropertyName("code")] 
    public string? Code { get; set; }
}

public sealed class VerifyEmailCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IEmailCommandService emailCommandService,
    IVerificationQueryService verificationQueryService,
    IVerificationCommandService verificationCommandService,
    ICodeManager codeManager) : IRequestHandler<VerifyEmailCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IEmailCommandService _emailCommandService = emailCommandService;
    private readonly IVerificationQueryService _verificationQueryService = verificationQueryService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;
    private readonly ICodeManager _codeManager = codeManager;

    public async Task<Result> Handle(VerifyEmailCommand request,
        CancellationToken cancellationToken = default)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new ValidationException("Code is requied");
        
        var codeEntity = await _codeManager.FindByCodeAsync(user, request.Code, cancellationToken);
        if (codeEntity is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Code is Invalid"
            });
        }

        var consumeResult = await _codeManager.ConsumeAsync(codeEntity, cancellationToken);
        if (!consumeResult.Succeeded) return consumeResult;
        
        var verificationRequest = await _verificationQueryService.GetByIdAsync(user.Id,
            request.VerificationId, cancellationToken);

        if (verificationRequest is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'verification_id' is invalid"
            });
        }
        
        if (verificationRequest.Status != VerificationStatus.Pending)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid verification request"
            });
        }

        var verificationResult = await _verificationCommandService.ConsumeAsync(verificationRequest, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email is required");
        
        return await _emailCommandService.VerifyAsync(user.Id, request.Email, cancellationToken);
    }
}

public sealed class VerifyEmailCommandValidator : IRequestValidator<VerifyEmailCommand>
{
    public async ValueTask<Result> Validate(VerifyEmailCommand request, CancellationToken cancellationToken)
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
        
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'current_email' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}