using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Server.Common.Messaging;
using eSecurity.Server.Common.Messaging.Messages.Email;
using eSecurity.Server.Common.Messaging.Messages.Sms;
using eSecurity.Server.Security.Authorization.Access.Codes;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Messaging;

namespace eSecurity.Server.Features.Verification.Commands;

public record ResendCodeCommand(ResendCodeRequest Request) : IRequest<Result>;

public class ResendCodeCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IMessageService messageService,
    IOptions<CodeOptions> options) : IRequestHandler<ResendCodeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IMessageService _messageService = messageService;
    private readonly CodeOptions _options = options.Value;
    public async Task<Result> Handle(ResendCodeCommand request, CancellationToken cancellationToken)
    {
        ResendCodeResponse? response;

        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        if (user.CodeResendAttempts >= _options.MaxCodeResendAttempts)
        {
            response = new ResendCodeResponse()
            {
                CodeResendAttempts = user.CodeResendAttempts,
                MaxCodeResendAttempts = _options.MaxCodeResendAttempts,
                CodeResendAvailableDate = user.CodeResendAvailableDate
            };

            return Results.Ok(response);
        }

        user.CodeResendAttempts += 1;

        if (user.CodeResendAttempts == _options.MaxCodeResendAttempts)
        {
            user.CodeResendAvailableDate = DateTimeOffset.UtcNow.AddMinutes(
                _options.CodeResendUnavailableTime);
        }

        var userUpdateResult = await _userManager.UpdateAsync(user, cancellationToken);
        if (!userUpdateResult.Succeeded) return userUpdateResult;

        var sender = request.Request.Sender;
        var action = request.Request.Action;
        var purpose = request.Request.Purpose;
        var payload = request.Request.Payload;

        var code = await _codeManager.GenerateAsync(user, sender, action, purpose, cancellationToken);
        payload["Code"] = code; 
        payload["UserName"] = user.Username;

        Message? message = sender switch
        {
            SenderType.Email => new VerificationEmailMessage(),
            SenderType.Sms => new VerificationSmsMessage(),
            _ => null
        };
        
        if (message is null) return Results.BadRequest("Invalid message type.");

        message.Initialize(payload);
        await _messageService.SendMessageAsync(sender, message, cancellationToken);

        response = new ResendCodeResponse()
        {
            CodeResendAttempts = user.CodeResendAttempts,
            MaxCodeResendAttempts = _options.MaxCodeResendAttempts,
            CodeResendAvailableDate = user.CodeResendAvailableDate
        };
        
        return Results.Ok(response);
    }
}