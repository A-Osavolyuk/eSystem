using eSecurity.Security.Authentication.TwoFactor;
using eSecurity.Security.Identity.User;
using eSystem.Core.Security.Authentication.TwoFactor;

namespace eSecurity.Features.TwoFactor.Commands;

public record PreferMethodCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
    public TwoFactorMethod PreferredMethod { get; set; }
}

public class PreferMethodCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager) : IRequestHandler<PreferMethodCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;

    public async Task<Result> Handle(PreferMethodCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}");
        
        var result = await twoFactorManager.PreferAsync(user, request.PreferredMethod, cancellationToken);
        return result;
    }
}