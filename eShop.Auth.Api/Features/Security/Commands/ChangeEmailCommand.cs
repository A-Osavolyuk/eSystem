using eShop.Domain.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ChangeEmailCommand(ChangeEmailRequest Request) : IRequest<Result>;

internal sealed class RequestChangeEmailCommandHandler(
    IMessageService messageService,
    IConfiguration configuration,
    ICodeManager codeManager,
    IUserManager userManager) : IRequestHandler<ChangeEmailCommand, Result>
{
    private readonly IMessageService messageService = messageService;
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly string frontendUri = configuration["Configuration:General:Frontend:Clients:BlazorServer:Uri"]!;

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

        await messageService.SendMessageAsync("email:email-change", new ChangeEmailMessage()
        {
            Code = oldEmailCode,
            NewEmail = request.Request.NewEmail,
            Credentials = new EmailCredentials()
            {
                To = request.Request.CurrentEmail,
                Subject = "Email change (step one)",
                UserName = request.Request.CurrentEmail,
            }
        }, cancellationToken);

        await messageService.SendMessageAsync("email:email-verification", new EmailVerificationMessage()
        {
            Code = newEmailCode,
            Credentials = new EmailCredentials()
            {
                UserName = request.Request.CurrentEmail,
                Subject = "Email change (step two)",
                To = request.Request.NewEmail,
            }
        }, cancellationToken);

        return Result.Success("We have sent a letter with instructions to your current and new email addresses");
    }
}