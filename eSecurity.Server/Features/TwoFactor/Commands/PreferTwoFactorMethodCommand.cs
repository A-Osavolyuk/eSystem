using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.TwoFactor.Commands;

public record PreferTwoFactorMethodCommand(PreferTwoFactorMethodRequest Request) : IRequest<Result>;

public class PreferMethodCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager) : IRequestHandler<PreferTwoFactorMethodCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;

    public async Task<Result> Handle(PreferTwoFactorMethodCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        
        var result = await twoFactorManager.PreferAsync(user, request.Request.PreferredMethod, cancellationToken);
        return result;
    }
}