using eSecurity.Core.Common.Requests;
using eSecurity.Server.Common.Messaging;
using eSecurity.Server.Common.Messaging.Messages.Email;
using eSecurity.Server.Common.Messaging.Messages.Sms;
using eSecurity.Server.Security.Authorization.Codes;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Verification.Commands;

public record SendCodeCommand(SendCodeRequest Request) : IRequest<Result>;

public class SendCodeCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IMessageService messageService,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<SendCodeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IMessageService _messageService = messageService;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(SendCodeCommand request, CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid request."
            });
        }
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "User not found."
            });
        }

        var sender = request.Request.Sender;
        var payload = request.Request.Payload;
        var code = await _codeManager.CreateAsync(user, sender, cancellationToken);

        payload["Code"] = code;
        payload["UserName"] = user.Username;

        Message? message = sender switch
        {
            SenderType.Email => new CodeEmailMessage(),
            SenderType.Sms => new CodeSmsMessage(),
            _ => null
        };

        if (message is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid message type."
            });
        }

        message.Initialize(payload);
        await _messageService.SendMessageAsync(sender, message, cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }
}