using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ConfirmForgotPasswordCommand(ConfirmForgotPasswordRequest Request) : IRequest<Result>;

public sealed class ConfirmForgotPasswordCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager) : IRequestHandler<ConfirmForgotPasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(ConfirmForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound("User not found.");
        }
        
        var result = await codeManager.VerifyAsync(user, request.Request.Code, SenderType.Email, 
            CodeType.Reset, CodeResource.Password, cancellationToken);

        return result;
    }
}