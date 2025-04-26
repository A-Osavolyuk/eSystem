using eShop.Application;
using eShop.Domain.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record VerifyEmailCommand(VerifyEmailRequest Request) : IRequest<Result>;

internal sealed class VerifyEmailCommandHandler(
    ISecurityManager securityManager,
    UserManager<UserEntity> userManager,
    IMessageService messageService,
    CartClient client) : IRequestHandler<VerifyEmailCommand, Result>
{
    private readonly ISecurityManager securityManager = securityManager;
    private readonly UserManager<UserEntity> userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly CartClient client = client;

    public async Task<Result> Handle(VerifyEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        var confirmResult = await securityManager.VerifyEmailAsync(user, request.Request.Code);

        if (!confirmResult.Succeeded)
        {
            return confirmResult;
        }

        await messageService.SendMessageAsync("email-verified", new EmailVerifiedMessage()
        {
            To = request.Request.Email,
            Subject = "Email verified",
            UserName = user.UserName!
        }, cancellationToken);

        var response = await client.InitiateUserAsync(new InitiateUserRequest() { UserId = user.Id.ToString() });

        if (!response.IsSucceeded)
        {
            return Results.InternalServerError(response.Message);
        }

        return Result.Success("Your email address was successfully confirmed.");
    }
}