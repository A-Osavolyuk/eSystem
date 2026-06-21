using eSecurity.Core.Requests.Email.Reset;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Email.Reset;

public sealed record ResetEmailCommand(ResetEmailRequest Request) : IRequest<Result>;

public sealed class ResetEmailCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IEmailCommandService emailCommandService,
    IVerificationQueryService verificationQueryService,
    IVerificationCommandService verificationCommandService,
    ICodeManager codeManager) : IRequestHandler<ResetEmailCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IEmailCommandService _emailCommandService = emailCommandService;
    private readonly IVerificationQueryService _verificationQueryService = verificationQueryService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;
    private readonly ICodeManager _codeManager = codeManager;

    public async Task<Result> Handle(ResetEmailCommand request, CancellationToken cancellationToken = default)
    {
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        var user = userResult.GetValue();

        if (string.IsNullOrWhiteSpace(request.Request.Code))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidEmail,
                Description = "'code' is required"
            });
        }
        
        if (string.IsNullOrWhiteSpace(request.Request.CurrentEmail))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidEmail,
                Description = "'current_email' is required"
            });
        }
        
        if (string.IsNullOrWhiteSpace(request.Request.NewEmail))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidEmail,
                Description = "'new_email' is required"
            });
        }
        
        var code = await _codeManager.FindByCodeAsync(user, request.Request.Code, cancellationToken);
        if (code is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'code' is invalid"
            });
        }

        var verificationRequest = await _verificationQueryService.GetByIdAsync(user.Id,
            request.Request.VerificationId, cancellationToken);

        if (verificationRequest is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'verification_id' is invalid"
            });
        }
        
        var consumeResult = await _codeManager.ConsumeAsync(code, cancellationToken);
        if (!consumeResult.Succeeded) return consumeResult;

        var verificationResult = await _verificationCommandService.ConsumeAsync(verificationRequest, cancellationToken);
        if (!verificationResult.Succeeded) return consumeResult;
        
        return await _emailCommandService.ResetAsync(user.Id, request.Request.CurrentEmail, 
            request.Request.NewEmail, cancellationToken);
    }
}