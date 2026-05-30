using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Requests;
using eSecurity.Idp.Common.Messaging.Email;
using eSecurity.Idp.Common.Messaging.Email.Builders;
using eSecurity.Idp.Common.Messaging.Sms;
using eSecurity.Idp.Common.Messaging.Sms.Builders;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Features.Verification.Commands;

public record SendCodeCommand(SendCodeRequest Request) : IRequest<Result>;

public class SendCodeCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    ISmsService smsService,
    IEmailService emailService,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<SendCodeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly ISmsService _smsService = smsService;
    private readonly IEmailService _emailService = emailService;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(SendCodeCommand request, CancellationToken cancellationToken)
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

        return Results.Success(SuccessCodes.Ok);
    }
}