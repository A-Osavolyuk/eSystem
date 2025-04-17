using eShop.Domain.Common.API;
using eShop.Domain.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ResendEmailVerificationCodeCommand(ResendEmailVerificationCodeRequest Request)
    : IRequest<Result>;

internal sealed class ResendEmailVerificationCodeCommandHandler(AppManager manager, IMessageService messageService)
    : IRequestHandler<ResendEmailVerificationCodeCommand, Result>
{
    private readonly AppManager manager = manager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(ResendEmailVerificationCodeCommand request,
        CancellationToken cancellationToken)
    {
        var user = await manager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with email: {request.Request.Email}"
            });
        }

        string code;

        var entity = await manager.SecurityManager.FindCodeAsync(user.Email!, Verification.VerifyEmail);

        if (entity is null || entity.ExpireDate < DateTime.UtcNow)
        {
            code = await manager.SecurityManager.GenerateVerificationCodeAsync(user.Email!,
                Verification.VerifyEmail);
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

        return Result.Success("Verification code was successfully resend");
    }
}