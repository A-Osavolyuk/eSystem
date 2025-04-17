using eShop.Domain.Common.API;
using eShop.Domain.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record RequestResetPasswordCommand(ResetPasswordRequest Request)
    : IRequest<Result>;

internal sealed class RequestResetPasswordCommandHandler(
    AppManager appManager,
    IMessageService messageService,
    IConfiguration configuration) : IRequestHandler<RequestResetPasswordCommand, Result>
{
    private readonly AppManager appManager = appManager;
    private readonly IMessageService messageService = messageService;
    private readonly IConfiguration configuration = configuration;
    private readonly string frontendUri = configuration["Configuration:General:Frontend:Clients:BlazorServer:Uri"]!;

    public async Task<Result> Handle(RequestResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with email {request.Request.Email}."
            });
        }

        var code = await appManager.SecurityManager.GenerateVerificationCodeAsync(user.Email!,
            Verification.ResetPassword);

        await messageService.SendMessageAsync("password-reset", new ResetPasswordMessage()
        {
            To = request.Request.Email,
            Subject = "Password reset",
            Code = code,
            UserName = user.UserName!
        });

        return Result.Success($"You have to confirm password reset. " +
                              $"We have sent an email with instructions to your email address.");
    }
}