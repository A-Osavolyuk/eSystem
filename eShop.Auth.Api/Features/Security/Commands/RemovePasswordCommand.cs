using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record RemovePasswordCommand(RemovePasswordRequest Request) : IRequest<Result>;

public class RemovePasswordCommandHandler(
    IUserManager userManager,
    ILoginMethodManager loginMethodManager) : IRequestHandler<RemovePasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ILoginMethodManager loginMethodManager = loginMethodManager;

    public async Task<Result> Handle(RemovePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");

        if (!user.HasLoginMethod(LoginType.Password))
            return Results.BadRequest("Sign-in with password is already disabled");

        if (!user.HasLoginMethod(LoginType.OAuth) && !user.HasLoginMethod(LoginType.Passkey))
            return Results.BadRequest("You need to configure sign-in with passkey or linked external account.");

        var method = user.GetLoginMethod(LoginType.Password);
        var methodResult = await loginMethodManager.RemoveAsync(method, cancellationToken);
        if (!methodResult.Succeeded) return methodResult;

        var result = await userManager.RemovePasswordAsync(user, cancellationToken);
        return result;
    }
}