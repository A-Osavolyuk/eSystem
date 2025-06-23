using eShop.Domain.Common.Messaging;
using eShop.Domain.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record VerifyEmailCommand(VerifyEmailRequest Request) : IRequest<Result>;

public sealed class VerifyEmailCommandHandler(
    IUserManager userManager,
    IMessageService messageService,
    ICodeManager codeManager) : IRequestHandler<VerifyEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(VerifyEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }
        
        var result = await codeManager.VerifyAsync(user, request.Request.Code, CodeType.Verify, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        var confirmResult = await userManager.ConfirmEmailAsync(user, cancellationToken);

        if (!confirmResult.Succeeded)
        {
            return confirmResult;
        }

        await messageService.SendMessageAsync(SenderType.Email, "email-verification", 
            null,
            new EmailCredentials()
            {
                To = request.Request.Email,
                Subject = "Email verification",
                UserName = user.UserName!
            }, cancellationToken);

        return Result.Success("Your email address was successfully confirmed.");
    }
}