using eShop.Domain.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record VerifyEmailCommand(VerifyEmailRequest Request) : IRequest<Result>;

internal sealed class VerifyEmailCommandHandler(
    ISecurityManager securityManager,
    IUserManager userManager,
    IMessageService messageService) : IRequestHandler<VerifyEmailCommand, Result>
{
    private readonly ISecurityManager securityManager = securityManager;
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(VerifyEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        var confirmResult = await securityManager.ConfirmEmailAsync(user, request.Request.Code);

        if (!confirmResult.Succeeded)
        {
            return confirmResult;
        }

        await messageService.SendMessageAsync("email:email-verified", new EmailVerifiedMessage()
        {
            To = request.Request.Email,
            Subject = "Email verified",
            UserName = user.UserName!
        }, cancellationToken);

        return Result.Success("Your email address was successfully confirmed.");
    }
}