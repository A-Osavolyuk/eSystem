using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record EnableMethodCommand(EnableMethodRequest Request) : IRequest<Result>;

public class EnableMethodCommandHandler(
    IUserManager userManager,
    ILoginMethodManager loginMethodManager) : IRequestHandler<EnableMethodCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ILoginMethodManager loginMethodManager = loginMethodManager;

    public async Task<Result> Handle(EnableMethodCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        if (user.HasLoginMethod(request.Request.Type)) return Results.BadRequest("Method is already enabled");

        var result = await loginMethodManager.CreateAsync(user, request.Request.Type, cancellationToken);
        return result;
    }
}