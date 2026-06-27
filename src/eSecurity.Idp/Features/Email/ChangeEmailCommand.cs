using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Email;

public sealed class ChangeEmailCommand : IRequest<Result>
{
    [JsonPropertyName("verification_id")]
    public Guid VerificationId { get; set; }
    
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    
    [JsonPropertyName("new_email")]
    public string? NewEmail { get; set; }
    
    [JsonPropertyName("current_email")]
    public string? CurrentEmail { get; set; }
}

public sealed class ChangeEmailCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IVerificationQueryService verificationQueryService,
    IVerificationCommandService verificationCommandService,
    ICodeQueryService codeQueryService,
    ICodeCommandService codeCommandService,
    IEmailCommandService emailCommandService) : IRequestHandler<ChangeEmailCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IVerificationQueryService _verificationQueryService = verificationQueryService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;
    private readonly ICodeQueryService _codeQueryService = codeQueryService;
    private readonly ICodeCommandService _codeCommandService = codeCommandService;
    private readonly IEmailCommandService _emailCommandService = emailCommandService;

    public async Task<Result> Handle(ChangeEmailCommand request, CancellationToken cancellationToken = default)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new ValidationException("Code is required");
        
        var code = await _codeQueryService.GetByCodeAsync(user.Id, request.Code, cancellationToken);
        if (code is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'code' is invalid"
            });
        }

        var codeResult = await _codeCommandService.ConsumeAsync(code, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;
        
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

        if (string.IsNullOrWhiteSpace(request.CurrentEmail))
            throw new ValidationException("CurrentEmail is required");

        if (string.IsNullOrWhiteSpace(request.NewEmail))
            throw new ValidationException("NewEmail is required");
        
        return await _emailCommandService.ChangeAsync(user.Id, request.CurrentEmail, 
            request.NewEmail, cancellationToken);
    }
}

public sealed class ChangeEmailCommandValidator : IRequestValidator<ChangeEmailCommand>
{
    public async ValueTask<Result> Validate(ChangeEmailCommand request, CancellationToken cancellationToken)
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
        
        if (string.IsNullOrWhiteSpace(request.CurrentEmail))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'current_email' is required"
            });
        }
        
        if (string.IsNullOrWhiteSpace(request.NewEmail))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'new_email' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}