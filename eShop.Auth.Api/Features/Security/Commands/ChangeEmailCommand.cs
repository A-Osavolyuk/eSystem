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

        var oldEmailCode = await codeManager.GenerateAsync(user, SenderType.Email, CodeType.Current, cancellationToken);
        var newEmailCode = await codeManager.GenerateAsync(user, SenderType.Email, CodeType.New, cancellationToken);

        await messageService.SendMessageAsync(SenderType.Email, "email/change",
            new
            {
                Code = oldEmailCode,
                NewEmail = request.Request.NewEmail,
            },
            new EmailCredentials()
            {
                To = user.Email,
                Subject = "Email change (step one)",
                UserName = user.UserName,
            }, cancellationToken);

        await messageService.SendMessageAsync(SenderType.Email, "email/verification",
            new
            {
                Code = newEmailCode,
            },
            new EmailCredentials()
            {
                To = request.Request.NewEmail,
                Subject = "Email verification (step two)",
                UserName = user.UserName,
            }, cancellationToken);

        return Result.Success();
    }
}