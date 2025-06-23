using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ConfirmResetPasswordCommand(ConfirmPasswordResetRequest Request)
    : IRequest<Result>;

public sealed class ConfirmResetPasswordCommandHandler(
    IUserManager userManager)
    : IRequestHandler<ConfirmResetPasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(ConfirmResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        var resetResult = await userManager.ResetPasswordAsync(user, request.Request.Code, 
            request.Request.NewPassword, cancellationToken);

        return resetResult;
    }
}