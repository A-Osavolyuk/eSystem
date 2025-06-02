using eShop.Domain.Common.Messaging;
using eShop.Domain.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record RequestResetPasswordCommand(ResetPasswordRequest Request)
    : IRequest<Result>;

internal sealed class RequestResetPasswordCommandHandler(
    IUserManager userManager,
    IMessageService messageService,
    ICodeManager codeManager) : IRequestHandler<RequestResetPasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(RequestResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        var code = await codeManager.GenerateAsync(user, CodeType.Reset, cancellationToken);

        await messageService.SendMessageAsync(SenderType.Email, MessagePath.ResetPassword, 
            new
            {
                Code = code,
            },
            new EmailCredentials()
            {
                To = request.Request.Email,
                Subject = "Email verification",
                UserName = user.UserName!
            }, cancellationToken);

        return Result.Success($"You have to confirm password reset. " +
                              $"We have sent an email with instructions to your email address.");
    }
}