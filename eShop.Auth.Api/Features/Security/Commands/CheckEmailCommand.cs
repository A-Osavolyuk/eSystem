using eShop.Domain.Requests.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record CheckEmailCommand(CheckEmailRequest Request) : IRequest<Result>;

public class CheckEmailCommandHandler(
    IUserManager userManager) : IRequestHandler<CheckEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(CheckEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var isTaken = await userManager.IsEmailTakenAsync(request.Request.Email, cancellationToken);
        if (isTaken) return Results.BadRequest("Email is already taken.");

        return Result.Success();
    }
}