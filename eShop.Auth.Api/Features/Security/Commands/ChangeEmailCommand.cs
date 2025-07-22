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
        
        var isTaken = await userManager.IsEmailTakenAsync(request.Request.NewEmail, cancellationToken);

        if (isTaken)
        {
            return Results.BadRequest("This email address is already taken");
        }

        var code = await codeManager.GenerateAsync(user, SenderType.Email, 
            CodeType.Current, CodeResource.Email, cancellationToken);
        
        var message = new ChangeEmailMessage()
        {
            Credentials = new ()
            {
                { "To", user.Email },
                { "Subject", "Email change (step one)" }
            },
            Payload = new()
            {
                { "Code", code },
                { "NewEmail", request.Request.NewEmail },
                { "UserName", user.UserName },
            }
        };
        
        await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

        return Result.Success();
    }
}