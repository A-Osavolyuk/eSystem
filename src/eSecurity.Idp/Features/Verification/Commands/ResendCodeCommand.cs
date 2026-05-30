using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Identity.Options;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Requests;
using eSecurity.Core.Responses;
using eSecurity.Idp.Common.Messaging.Email;
using eSecurity.Idp.Common.Messaging.Email.Builders;
using eSecurity.Idp.Common.Messaging.Sms;
using eSecurity.Idp.Common.Messaging.Sms.Builders;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Features.Verification.Commands;

public record ResendCodeCommand(ResendCodeRequest Request) : IRequest<Result>;

public class ResendCodeCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IOptions<CodeOptions> options,
    IEmailService emailService,
    ISmsService smsService,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<ResendCodeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IEmailService _emailService = emailService;
    private readonly ISmsService _smsService = smsService;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly CodeOptions _options = options.Value;
    
    public async Task<Result> Handle(ResendCodeCommand request, CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid request."
            });
        }
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "User not found."
            });
        }

        if (user.CodeResendAttempts >= _options.MaxCodeResendAttempts)
        {
            return Results.Success(SuccessCodes.Ok, new ResendCodeResponse
            {
                CodeResendAttempts = user.CodeResendAttempts,
                MaxCodeResendAttempts = _options.MaxCodeResendAttempts,
                CodeResendAvailableDate = user.CodeResendAvailableDate
            });
        }

        user.CodeResendAttempts += 1;

        if (user.CodeResendAttempts == _options.MaxCodeResendAttempts)
        {
            user.CodeResendAvailableDate = DateTimeOffset.UtcNow.AddMinutes(
                _options.CodeResendUnavailableTime);
        }

        var userUpdateResult = await _userManager.UpdateAsync(user, cancellationToken);
        if (!userUpdateResult.Succeeded) return userUpdateResult;
        
        var code = await _codeManager.CreateAsync(user, request.Request.Sender, cancellationToken);
        if (request.Request.Sender is SenderType.Email)
        {
            var emailContext = new CodeVerificationEmailContext()
            {
                Code = code,
                Subject = request.Request.Payload["Subject"],
                To = request.Request.Payload["To"]
            };

            await _emailService.SendAsync(emailContext, cancellationToken);
        }
        else
        {
            var smsContext = new CodeVerificationSmsContext()
            {
                Code = code,
                To = request.Request.Payload["To"]
            };

            await _smsService.SendAsync(smsContext, cancellationToken);
        }

        var response = new ResendCodeResponse
        {
            CodeResendAttempts = user.CodeResendAttempts,
            MaxCodeResendAttempts = _options.MaxCodeResendAttempts,
            CodeResendAvailableDate = user.CodeResendAvailableDate
        };
        
        return Results.Success(SuccessCodes.Ok, response);
    }
}