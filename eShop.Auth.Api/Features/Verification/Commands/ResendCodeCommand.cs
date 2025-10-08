using eShop.Auth.Api.Messages.Email;
using eShop.Auth.Api.Messages.Sms;
using eShop.Domain.Requests.Auth;
using eShop.Domain.Responses.Auth;

namespace eShop.Auth.Api.Features.Verification.Commands;

public record ResendCodeCommand(ResendCodeRequest Request) : IRequest<Result>;

public class ResendCodeCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IMessageService messageService,
    IdentityOptions identityOptions) : IRequestHandler<ResendCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;
    private readonly IdentityOptions identityOptions = identityOptions;
    public async Task<Result> Handle(ResendCodeCommand request, CancellationToken cancellationToken)
    {
        ResendCodeResponse? response;

        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        if (request.Request.Sender is SenderType.AuthenticatorApp)
        {
            return Result.Success("Code successfully sent. Please, check your authenticator app.");
        }

        if (user.CodeResendAttempts >= identityOptions.Code.MaxCodeResendAttempts)
        {
            response = new ResendCodeResponse()
            {
                CodeResendAttempts = user.CodeResendAttempts,
                MaxCodeResendAttempts = identityOptions.Code.MaxCodeResendAttempts,
                CodeResendAvailableDate = user.CodeResendAvailableDate
            };

            return Result.Success(response);
        }

        user.CodeResendAttempts += 1;

        if (user.CodeResendAttempts == identityOptions.Code.MaxCodeResendAttempts)
        {
            user.CodeResendAvailableDate = DateTimeOffset.UtcNow.AddMinutes(
                identityOptions.Code.CodeResendUnavailableTime);
        }

        var userUpdateResult = await userManager.UpdateAsync(user, cancellationToken);
        if (!userUpdateResult.Succeeded) return userUpdateResult;

        var sender = request.Request.Sender;
        var action = request.Request.Action;
        var purpose = request.Request.Purpose;
        var payload = request.Request.Payload;

        var code = await codeManager.GenerateAsync(user, sender, action, purpose, cancellationToken);
        payload["Code"] = code; 
        payload["UserName"] = user.Username;

        Message? message = sender switch
        {
            SenderType.Email => new VerificationCodeEmailMessage(),
            SenderType.Sms => new VerificationCodeSmsMessage(),
            _ => null
        };
        
        if (message is null) return Results.BadRequest("Invalid message type.");

        message.Initialize(payload);
        await messageService.SendMessageAsync(sender, message, cancellationToken);

        response = new ResendCodeResponse()
        {
            CodeResendAttempts = user.CodeResendAttempts,
            MaxCodeResendAttempts = identityOptions.Code.MaxCodeResendAttempts,
            CodeResendAvailableDate = user.CodeResendAvailableDate
        };
        
        return Result.Success(response);
    }
}