using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ResendEmailVerificationCodeCommand(ResendEmailVerificationCodeRequest Request)
    : IRequest<Result>;

public sealed class ResendEmailVerificationCodeCommandHandler(
    IUserManager userManager,
    IMessageService messageService,
    ICodeManager codeManager)
    : IRequestHandler<ResendEmailVerificationCodeCommand, Result>
{
    private readonly IMessageService messageService = messageService;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(ResendEmailVerificationCodeCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email: {request.Request.Email}");
        }

        string code;

        var entity = await codeManager.FindAsync(user, SenderType.Email, 
            CodeType.Verify, CodeResource.Email, cancellationToken);

        if (entity is null || entity.ExpireDate < DateTime.UtcNow)
        {
            code = await codeManager.GenerateAsync(user, SenderType.Email, 
                CodeType.Verify, CodeResource.Email, cancellationToken);
        }
        else
        {
            code = entity.Code;
        }

        var message = new VerifyEmailMessage()
        {
            Credentials = new ()
            {
                { "To", user!.Email },
                { "Subject", "Email verification" },
                { "UserName", user.Email },
            }, 
            Payload = new()
            {
                { "Code", code },
                { "UserName", user.UserName },
            }
        };
        
        await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

        return Result.Success("Verification code was successfully resend");
    }
}