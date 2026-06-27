using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Authentication.Password;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Password;

public sealed class ResetPasswordCommand : IRequest<Result>
{
    [JsonPropertyName("verification_id")]
    public Guid VerificationId { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("password")]
    public string? Password { get; set; }
}

public sealed class ResetPasswordCommandHandler(
    IUserQueryService userQueryService,
    IVerificationQueryService verificationQueryService,
    IPasswordCommandService passwordCommandService,
    IVerificationCommandService verificationCommandService) : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IVerificationQueryService _verificationQueryService = verificationQueryService;
    private readonly IPasswordCommandService _passwordCommandService = passwordCommandService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email is required");
        
        var user = await _userQueryService.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "User not found."
            });
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

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ValidationException("Password is required");
        
        return await _passwordCommandService.ResetAsync(user.Id, request.Password, cancellationToken);
    }
}

public sealed class ResetPasswordCommandValidator : IRequestValidator<ResetPasswordCommand>
{
    public async ValueTask<Result> Validate(ResetPasswordCommand request, CancellationToken cancellationToken)
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

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'password' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}