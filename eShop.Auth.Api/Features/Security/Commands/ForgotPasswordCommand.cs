using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ForgotPasswordCommand(ForgotPasswordRequest Request) : IRequest<Result>;

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
        if (user is null) return Results.NotFound($"Cannot find user with email {request.Request.Email}.");

        var code = await codeManager.GenerateAsync(user, SenderType.Email, CodeType.Reset, 
            CodeResource.Password, cancellationToken);

        var message = new ForgotPasswordMessage()
        {
            Credentials = new ()
            {
                { "To", user.Email },
                { "Subject", "Password reset" }
            },
            Payload = new()
            {
                { "Code", code },
                { "UserName", user.UserName }
            }
        };
        
        await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

        var response = new ForgotPasswordResponse()
        {
            UserId = user.Id
        };

        return Result.Success(response);
    }
}