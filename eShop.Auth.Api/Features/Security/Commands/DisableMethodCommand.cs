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
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        if (!user.HasLoginMethod(request.Request.Type)) return Results.BadRequest("Method is already disabled.");
        
        var method = user.GetLoginMethod(request.Request.Type);
        var result = await loginMethodManager.RemoveAsync(method, cancellationToken);
        return result;
    }
}