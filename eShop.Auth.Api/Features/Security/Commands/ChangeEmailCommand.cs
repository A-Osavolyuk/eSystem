using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ChangeEmailCommand(ChangeEmailRequest Request) : IRequest<Result>;

public sealed class RequestChangeEmailCommandHandler(
    IMessageService messageService,
    ICodeManager codeManager,
    IUserManager userManager) : IRequestHandler<ChangeEmailCommand, Result>
{
    private readonly IMessageService messageService = messageService;
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(ChangeEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        }
        
        var isEmailTaken = await userManager.CheckEmailAsync(request.Request.NewEmail, cancellationToken);

        if (isEmailTaken)
        {
            return Results.BadRequest("This email address is already taken");
        }

        var oldEmailCode = await codeManager.GenerateAsync(user, SenderType.Email, CodeType.Current, 
            CodeResource.Email, cancellationToken);
        var newEmailCode = await codeManager.GenerateAsync(user, SenderType.Email, CodeType.New, 
            CodeResource.Email, cancellationToken);
        
        var stepOneMessage = new ChangeEmailMessage()
        {
            Credentials = new ()
            {
                { "To", user.Email },
                { "Subject", "Email change (step one)" }
            },
            Payload = new()
            {
                { "Code", oldEmailCode },
                { "NewEmail", request.Request.NewEmail },
                { "UserName", user.UserName },
            }
        };
        
        await messageService.SendMessageAsync(SenderType.Email, stepOneMessage, cancellationToken);
        
        var stepTwoMessage = new VerifyEmailMessage()
        {
            Credentials = new ()
            {
                { "To", request.Request.NewEmail },
                { "Subject", "Email verification (step two)" }
            }, 
            Payload = new()
            {
                { "Code", newEmailCode },
                { "UserName", user.UserName },
            }
        };
        
        await messageService.SendMessageAsync(SenderType.Email, stepTwoMessage, cancellationToken);

        return Result.Success();
    }
}