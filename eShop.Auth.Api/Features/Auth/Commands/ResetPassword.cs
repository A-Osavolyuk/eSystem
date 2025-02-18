namespace eShop.Auth.Api.Features.Auth.Commands;

internal sealed record RequestResetPasswordCommand(ResetPasswordRequest Request)
    : IRequest<Result<ResetPasswordResponse>>;

internal sealed class RequestResetPasswordCommandHandler(
    AppManager appManager,
    IMessageService messageService,
    IConfiguration configuration) : IRequestHandler<RequestResetPasswordCommand, Result<ResetPasswordResponse>>
{
    private readonly AppManager appManager = appManager;
    private readonly IMessageService messageService = messageService;
    private readonly IConfiguration configuration = configuration;
    private readonly string frontendUri = configuration["Configuration:General:Frontend:Clients:BlazorServer:Uri"]!;

    public async Task<Result<ResetPasswordResponse>> Handle(RequestResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with email {request.Request.Email}."));
        }

        var code = await appManager.SecurityManager.GenerateVerificationCodeAsync(user.Email!,
            VerificationCodeType.ResetPassword);

        await messageService.SendMessageAsync("password-reset", new ResetPasswordMessage()
        {
            To = request.Request.Email,
            Subject = "Password reset",
            Code = code,
            UserName = user.UserName!
        });

        return new(new ResetPasswordResponse()
        {
            Message = $"You have to confirm password reset. " +
                      $"We have sent an email with instructions to your email address."
        });
    }
}