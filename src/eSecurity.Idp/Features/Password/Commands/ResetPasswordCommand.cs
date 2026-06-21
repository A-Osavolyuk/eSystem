using eSecurity.Idp.Security.Authentication.Password;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Requests;
using eSecurity.Core.Security.Authorization.Verification;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Password.Commands;

public sealed record ResetPasswordCommand(ResetPasswordRequest Request) : IRequest<Result>;

public sealed class ResetPasswordCommandHandler(
    IUserQueryService userQueryService,
    IPasswordManager passwordManager,
    IVerificationQueryService verificationQueryService,
    IVerificationCommandService verificationCommandService) : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly IVerificationQueryService _verificationQueryService = verificationQueryService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userQueryService.GetByEmailAsync(request.Request.Email, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "User not found."
            });
        }

        if (!await _passwordManager.HasAsync(user, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidPassword,
                Description = "User does not have a password."
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

        var result = await _passwordManager.ResetAsync(user, request.Request.Password, cancellationToken);
        return result;
    }
}