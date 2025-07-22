using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record VerifyNewEmailCommand(VerifyNewEmailRequest Request) : IRequest<Result>;

public class VerifyNewEmailCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IMessageService messageService) : IRequestHandler<VerifyNewEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(VerifyNewEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        }
        
        var isTaken = await userManager.IsEmailTakenAsync(request.Request.NewEmail, cancellationToken);

        if (isTaken)
        {
            return Results.BadRequest("This email address is already taken");
        }
        
        var codeResult = await codeManager.VerifyAsync(user, request.Request.Code, 
            SenderType.Email, CodeType.Current, CodeResource.Email, cancellationToken);

        if (!codeResult.Succeeded)
        {
            return codeResult;
        }
        
        var code = await codeManager.GenerateAsync(user, SenderType.Email, CodeType.New, 
            CodeResource.Email, cancellationToken);
        
        var message = new VerifyEmailMessage()
        {
            Credentials = new ()
            {
                { "To", request.Request.NewEmail },
                { "Subject", "Email verification (step two)" }
            }, 
            Payload = new()
            {
                { "Code", code },
                { "UserName", user.UserName },
            }
        };
        
        await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

        return Result.Success();
    }
}