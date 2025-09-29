using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record DisableMethodCommand(DisableMethodRequest Request) : IRequest<Result>;

public class DisableMethodCommandHandler(
    IUserManager userManager,
    ILoginMethodManager loginMethodManager) : IRequestHandler<DisableMethodCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ILoginMethodManager loginMethodManager = loginMethodManager;

    public async Task<Result> Handle(DisableMethodCommand request, CancellationToken cancellationToken)
    {
        var loginType = request.Request.Type;

        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        if (!user.HasLoginMethod(loginType)) return Results.BadRequest("Method is already disabled.");

        if (loginType == LoginType.Passkey
            && !user.HasLoginMethod(LoginType.Password)
            || !user.HasLoginMethod(LoginType.OAuth))
            return Results.BadRequest("You need to configure password sign-in or link external account");

        if (loginType == LoginType.Password
            && !user.HasLoginMethod(LoginType.Passkey)
            || !user.HasLoginMethod(LoginType.OAuth))
            return Results.BadRequest("You need to configure passkey sign-in or link external account");

        if (loginType == LoginType.OAuth
            && !user.HasLoginMethod(LoginType.Password)
            || !user.HasLoginMethod(LoginType.Passkey))
            return Results.BadRequest("You need to configure sign-in with password or passkey");

        var method = user.GetLoginMethod(loginType);
        var result = await loginMethodManager.RemoveAsync(method, cancellationToken);
        return result;
    }
}