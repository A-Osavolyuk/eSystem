using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ChangePasswordCommand(ChangePasswordRequest Request) : IRequest<Result>;

public sealed class ChangePasswordCommandHandler(
    IUserManager userManager) : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    async Task<Result> IRequestHandler<ChangePasswordCommand, Result>.Handle(ChangePasswordCommand request,
            CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        if (!user.HasPassword()) return Results.BadRequest("Cannot reset password, password was not provided.");

        var isCorrectPassword = await userManager.CheckPasswordAsync(user, request.Request.CurrentPassword, cancellationToken);
        if (!isCorrectPassword) return Results.BadRequest($"Wrong password.");
        
        var result = await userManager.ChangePasswordAsync(user, request.Request.NewPassword, cancellationToken);

        return result;
    }
}