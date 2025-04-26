using eShop.Domain.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ChangeEmailCommand(ChangeEmailRequest Request) : IRequest<Result>;

internal sealed class RequestChangeEmailCommandHandler(
    IMessageService messageService,
    IConfiguration configuration,
    ICodeManager codeManager,
    UserManager<UserEntity> userManager) : IRequestHandler<ChangeEmailCommand, Result>
{
    private readonly IMessageService messageService = messageService;
    private readonly UserManager<UserEntity> userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly string frontendUri = configuration["Configuration:General:Frontend:Clients:BlazorServer:Uri"]!;

    public async Task<Result> Handle(ChangeEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.CurrentEmail);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.CurrentEmail}");
        }

        var destination = new DestinationSet()
        {
            Current = request.Request.CurrentEmail,
            Next = request.Request.NewEmail
        };

        var oldEmailCode = await codeManager.GenerateAsync(user, Verification.OldEmail, cancellationToken);
        var newEmailCode = await codeManager.GenerateAsync(user, Verification.NewEmail, cancellationToken);

        await messageService.SendMessageAsync("email-change", new ChangeEmailMessage()
        {
            Code = oldEmailCode,
            To = request.Request.CurrentEmail,
            Subject = "Email change (step one)",
            UserName = request.Request.CurrentEmail,
            NewEmail = request.Request.NewEmail,
        }, cancellationToken);

        await messageService.SendMessageAsync("email-verification", new EmailVerificationMessage()
        {
            Code = newEmailCode,
            UserName = request.Request.CurrentEmail,
            Subject = "Email change (step two)",
            To = request.Request.NewEmail,
        }, cancellationToken);

        return Result.Success("We have sent a letter with instructions to your current and new email addresses");
    }
}