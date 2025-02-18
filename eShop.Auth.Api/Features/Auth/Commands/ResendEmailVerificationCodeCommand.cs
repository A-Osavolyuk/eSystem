namespace eShop.Auth.Api.Features.Auth.Commands;

internal sealed record ResendEmailVerificationCodeCommand(ResendEmailVerificationCodeRequest Request)
    : IRequest<Result<ResendEmailVerificationCodeResponse>>;

internal sealed class ResendEmailVerificationCodeCommandHandler(AppManager manager, IMessageService messageService)
    : IRequestHandler<ResendEmailVerificationCodeCommand, Result<ResendEmailVerificationCodeResponse>>
{
    private readonly AppManager manager = manager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result<ResendEmailVerificationCodeResponse>> Handle(ResendEmailVerificationCodeCommand request,
        CancellationToken cancellationToken)
    {
        var user = await manager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with email: {request.Request.Email}"));
        }

        string code;

        var entity = await manager.SecurityManager.FindCodeAsync(user.Email!, VerificationCodeType.VerifyEmail);

        if (entity is null || entity.ExpireDate < DateTime.UtcNow)
        {
            code = await manager.SecurityManager.GenerateVerificationCodeAsync(user.Email!,
                VerificationCodeType.VerifyEmail);
        }
        else
        {
            code = entity.Code;
        }

        await messageService.SendMessageAsync("email-verification", new EmailVerificationMessage()
        {
            To = request.Request.Email,
            Code = code,
            Subject = "Email verification",
            UserName = user.UserName!
        });

        return new Result<ResendEmailVerificationCodeResponse>(new ResendEmailVerificationCodeResponse()
        {
            Message = "Verification code was successfully resend"
        });
    }
}