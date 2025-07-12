using eShop.Domain.Abstraction.Messaging.Email;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ForgotPasswordCommand(ForgotPasswordRequest Request)
    : IRequest<Result>;

public sealed class ForgotPasswordCommandHandler(
    IUserManager userManager,
    IMessageService messageService,
    ICodeManager codeManager) : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        var code = await codeManager.GenerateAsync(user, SenderType.Email, CodeType.Reset, cancellationToken);

        await messageService.SendMessageAsync(SenderType.Email, "password-reset", 
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

        var response = new ForgotPasswordResponse()
        {
            UserId = user.Id
        };

        return Result.Success(response);
    }
}