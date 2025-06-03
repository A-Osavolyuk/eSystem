using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ChangePasswordCommand(ChangePasswordRequest Request)
    : IRequest<Result>;

internal sealed class ChangePasswordCommandHandler(
    IUserManager userManager) : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    async Task<Result>
        IRequestHandler<ChangePasswordCommand, Result>.Handle(ChangePasswordCommand request,
            CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);
        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        var isCorrectPassword = await userManager.CheckPasswordAsync(user, request.Request.OldPassword, cancellationToken);
        if (!isCorrectPassword)
        {
            return Results.BadRequest($"Wrong password.");
        }

        var result = await userManager.ChangePasswordAsync(user, request.Request.OldPassword, request.Request.NewPassword, cancellationToken);

        return result;
    }
}