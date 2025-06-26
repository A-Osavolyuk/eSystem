using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record ResetEmailCommand(ResetEmailRequest Request) : IRequest<Result>;

public class ResetEmailCommandHandler(
    ICodeManager codeManager,
    IMessageService messageService,
    IUserManager userManager) : IRequestHandler<ResetEmailCommand, Result>
{
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(ResetEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.Id, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.Id}");
        }

        var code = await codeManager.GenerateAsync(user, SenderType.Email, CodeType.Reset, cancellationToken);

        await messageService.SendMessageAsync(SenderType.Email, "email/reset",
            new
            {
                Code = code
            },
            new EmailCredentials()
            {
                To = request.Request.NewEmail,
                UserName = user.UserName,
                Subject = "Email reset"
            }, cancellationToken);

        return Result.Success();
    }
}