using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

public record EnableTwoFactorCommand(EnableTwoFactorRequest Request) : IRequest<Result>;

public class EnableTwoFactorCommandHandler(IUserManager userManager) : IRequestHandler<EnableTwoFactorCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        if (user.TwoFactorEnabled) return Results.BadRequest("2FA already enabled.");
        
        user.TwoFactorEnabled = true;
        var updateResult = await userManager.UpdateAsync(user, cancellationToken);
        if (updateResult.Succeeded) return updateResult;

        return Result.Success();
    }
}