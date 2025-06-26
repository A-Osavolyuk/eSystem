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

        var entity = await codeManager.FindAsync(user, SenderType.Email, CodeType.Verify, cancellationToken);

        if (entity is null || entity.ExpireDate < DateTime.UtcNow)
        {
            code = await codeManager.GenerateAsync(user, SenderType.Email, CodeType.Verify, cancellationToken);
        }
        else
        {
            code = entity.Code;
        }

        await messageService.SendMessageAsync(SenderType.Email, "email/verify", 
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

        return Result.Success("Verification code was successfully resend");
    }
}