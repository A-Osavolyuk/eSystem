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
    ICurrentUserAccessor currentUserAccessor,
    IUserResendAttemptsService resendAttemptsService,
    ICodeManager codeManager,
    IOptions<CodeOptions> options,
    IEmailService emailService,
    ISmsService smsService,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<ResendCodeCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IUserResendAttemptsService _resendAttemptsService = resendAttemptsService;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IEmailService _emailService = emailService;
    private readonly ISmsService _smsService = smsService;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly CodeOptions _options = options.Value;
    
    public async Task<Result> Handle(ResendCodeCommand request, CancellationToken cancellationToken)
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

        if (user.ResendAttempts >= _options.MaxCodeResendAttempts)
        {
            return Results.Success(SuccessCodes.Ok, new ResendCodeResponse
            {
                CodeResendAttempts = user.ResendAttempts,
                MaxCodeResendAttempts = _options.MaxCodeResendAttempts,
                CodeResendAvailableDate = user.ResendAvailableAt
            });
        }

        var dueTime = TimeSpan.FromMinutes(_options.CodeResendUnavailableTime);
        var userUpdateResult = await _resendAttemptsService.ResetAttemptsAsync(user, dueTime, cancellationToken);
        if (!userUpdateResult.Succeeded) return userUpdateResult;
        
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

        var response = new ResendCodeResponse
        {
            CodeResendAttempts = user.ResendAttempts,
            MaxCodeResendAttempts = _options.MaxCodeResendAttempts,
            CodeResendAvailableDate = user.ResendAvailableAt
        };
        
        return Results.Success(SuccessCodes.Ok, response);
    }
}