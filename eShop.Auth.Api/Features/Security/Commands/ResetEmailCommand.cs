using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record ResetEmailCommand(ResetEmailRequest Request) : IRequest<Result>;

public class ResetEmailCommandHandler(
    ICodeManager codeManager,
    IMessageService messageService,
    IUserManager userManager,
    IdentityOptions identityOptions) : IRequestHandler<ResetEmailCommand, Result>
{
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;
    private readonly IUserManager userManager = userManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(ResetEmailCommand request, CancellationToken cancellationToken)
    {
        var newEmail = request.Request.NewEmail;
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        }
        
        if (identityOptions.Account.RequireUniqueEmail)
        {
            var isTaken = await userManager.IsEmailTakenAsync(request.Request.NewEmail, cancellationToken);

            if (isTaken)
            {
                return Results.BadRequest("This email address is already taken");
            }
        }

        var code = await codeManager.GenerateAsync(user, SenderType.Email, CodeType.Reset, 
            CodeResource.Email, cancellationToken);
        
        var message = new ResetEmailMessage
        {
            Payload = new()
            {
                { "Code", code },
                { "UserName", user.UserName },
            },
            Credentials = new Dictionary<string, string>()
            {
                { "To", newEmail },
                { "Subject", "Email reset" }
            }
        };
        
        await messageService.SendMessageAsync(SenderType.Email,  message, cancellationToken);

        return Result.Success();
    }
}