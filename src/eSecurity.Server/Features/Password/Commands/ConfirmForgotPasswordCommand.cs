using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authorization.Codes;
using eSecurity.Server.Security.Authorization.Verification;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;

namespace eSecurity.Server.Features.Password.Commands;

public sealed record ConfirmForgotPasswordCommand(ConfirmForgotPasswordRequest Request) : IRequest<Result>;

public sealed class ConfirmForgotPasswordCommandHandler(
    IUserManager userManager, 
    ICodeManager codeManager, 
    IVerificationManager verificationManager) : IRequestHandler<ConfirmForgotPasswordCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IVerificationManager _verificationManager = verificationManager;

    public async Task<Result> Handle(ConfirmForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Request.Email, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var code = await _codeManager.FindAsync(user, request.Request.Code, cancellationToken);
        if (code is null || code.ExpiredAt < DateTimeOffset.UtcNow) 
            return Results.NotFound("Invalid code.");

        var codeResult = await _codeManager.ConsumeAsync(code, cancellationToken);
        if (codeResult.Succeeded) return codeResult;

        var requestEntity = new VerificationRequestEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Purpose = PurposeType.Password,
            Action = ActionType.Reset,
            Status = VerificationStatus.Approved,
            Method = VerificationMethod.EmailOtp,
            ApprovedAt = DateTimeOffset.UtcNow,
            ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(15)
        };
        
        var verificationResult = await _verificationManager.CreateAsync(requestEntity, cancellationToken);
        if (verificationResult.Succeeded) return verificationResult;

        var response = new ConfirmForgotPasswordResponse { VerificationId = requestEntity.Id };
        return Results.Ok(response);
    }
}