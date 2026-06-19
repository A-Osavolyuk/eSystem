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
    ICurrentUserAccessor currentUserAccessor,
    ICodeManager codeManager,
    ISmsService smsService,
    IEmailService emailService,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<SendCodeCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly ISmsService _smsService = smsService;
    private readonly IEmailService _emailService = emailService;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(SendCodeCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        if (!userResult.TryGetValue(out var user))
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
            });
        }
        
        var codeResult = await _codeManager.CreateAsync(user, request.Request.Sender, cancellationToken);
        if (!codeResult.Succeeded)
        {
            var error = codeResult.GetError();
            return Results.ServerError(ServerErrorCode.InternalServerError, error);
        }

        if (!codeResult.TryGetValue(out var code))
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }
        
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