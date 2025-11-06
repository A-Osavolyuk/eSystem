using eSecurity.Messaging;
using eSecurity.Messaging.Messages.Email;
using eSecurity.Messaging.Messages.Sms;
using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Identity.Options;
using eSecurity.Security.Identity.User;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Authorization.Access;

namespace eSecurity.Features.Verification.Commands;

public class ResendCodeCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
    public required SenderType Sender { get; set; }
    public required ActionType Action { get; set; }
    public required PurposeType Purpose { get; set; }
    public required Dictionary<string, string> Payload { get; set; }
}

public class ResendCodeCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IMessageService messageService,
    IOptions<CodeOptions> options) : IRequestHandler<ResendCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;
    private readonly CodeOptions options = options.Value;
    public async Task<Result> Handle(ResendCodeCommand request, CancellationToken cancellationToken)
    {
        ResendCodeResponse? response;

        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        if (user.CodeResendAttempts >= options.MaxCodeResendAttempts)
        {
            response = new ResendCodeResponse()
            {
                CodeResendAttempts = user.CodeResendAttempts,
                MaxCodeResendAttempts = options.MaxCodeResendAttempts,
                CodeResendAvailableDate = user.CodeResendAvailableDate
            };

            return Result.Success(response);
        }

        user.CodeResendAttempts += 1;

        if (user.CodeResendAttempts == options.MaxCodeResendAttempts)
        {
            user.CodeResendAvailableDate = DateTimeOffset.UtcNow.AddMinutes(
                options.CodeResendUnavailableTime);
        }

        var userUpdateResult = await userManager.UpdateAsync(user, cancellationToken);
        if (!userUpdateResult.Succeeded) return userUpdateResult;

        var sender = request.Sender;
        var action = request.Action;
        var purpose = request.Purpose;
        var payload = request.Payload;

        var code = await codeManager.GenerateAsync(user, sender, action, purpose, cancellationToken);
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
        await messageService.SendMessageAsync(sender, message, cancellationToken);

        response = new ResendCodeResponse()
        {
            CodeResendAttempts = user.CodeResendAttempts,
            MaxCodeResendAttempts = options.MaxCodeResendAttempts,
            CodeResendAvailableDate = user.CodeResendAvailableDate
        };
        
        return Result.Success(response);
    }
}