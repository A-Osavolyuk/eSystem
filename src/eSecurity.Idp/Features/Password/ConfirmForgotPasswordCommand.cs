using System.Text.Json.Serialization;
using eSecurity.Core.Responses;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Password;

public sealed record ConfirmForgotPasswordCommand : IRequest<Result>
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("code")]
    public required string Code { get; set; }
}

public sealed class ConfirmForgotPasswordCommandHandler(
    IUserQueryService userQueryService, 
    ICodeManager codeManager,
    IVerificationCommandService verificationCommandService) : IRequestHandler<ConfirmForgotPasswordCommand, Result>
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;

    public async Task<Result> Handle(ConfirmForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userQueryService.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "User not found."
            });
        }

        var code = await _codeManager.FindByCodeAsync(user, request.Code, cancellationToken);
        if (code is null || code.ExpiredAt < DateTimeOffset.UtcNow)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Code not found."
            });
        }

        var codeResult = await _codeManager.ConsumeAsync(code, cancellationToken);
        if (codeResult.Succeeded) return codeResult;

        var requestEntity = new VerificationRequestEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Operation = OperationType.ResetPassword,
            Status = VerificationStatus.Approved,
            Method = VerificationMethod.EmailOtp,
            ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(15)
        };
        
        var verificationResult = await _verificationCommandService.CreateAsync(requestEntity, cancellationToken);
        if (verificationResult.Succeeded) return verificationResult;

        var response = new ConfirmForgotPasswordResponse { VerificationId = requestEntity.Id };
        return Results.Success(SuccessCodes.Ok, response);
    }
}