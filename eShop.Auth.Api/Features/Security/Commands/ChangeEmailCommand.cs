using eShop.Domain.Common.Messaging;
using eShop.Domain.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ChangeEmailCommand(ChangeEmailRequest Request) : IRequest<Result>;

internal sealed class RequestChangeEmailCommandHandler(
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
        var user = await userManager.FindByEmailAsync(request.Request.CurrentEmail, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.CurrentEmail}");
        }

        var oldEmailCode = await codeManager.GenerateAsync(user, CodeType.Current, cancellationToken);
        var newEmailCode = await codeManager.GenerateAsync(user, CodeType.New, cancellationToken);

        await messageService.SendMessageAsync(SenderType.Email, MessagePath.ChangeEmail, 
            new
            {
                Code = oldEmailCode,
                NewEmail = request.Request.NewEmail,
            },
            new EmailCredentials()
            {
                To = request.Request.CurrentEmail,
                Subject = "Email change (step one)",
                UserName = request.Request.CurrentEmail,
            }, cancellationToken);

        await messageService.SendMessageAsync(SenderType.Email, MessagePath.VerifyEmail, 
            new
            {
                Code = newEmailCode,
            },
            new EmailCredentials()
            {
                To = request.Request.CurrentEmail,
                Subject = "Email change (step one)",
                UserName = request.Request.CurrentEmail,
            }, cancellationToken);

        return Result.Success("We have sent a letter with instructions to your current and new email addresses");
    }
}