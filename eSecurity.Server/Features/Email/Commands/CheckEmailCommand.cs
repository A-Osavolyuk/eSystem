using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Email.Commands;

public record CheckEmailCommand(CheckEmailRequest Request) : IRequest<Result>;

public class CheckEmailCommandHandler(
    IUserManager userManager) : IRequestHandler<CheckEmailCommand, Result>
{
    private readonly IUserManager _userManager = userManager;

    public async Task<Result> Handle(CheckEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var isTaken = await _userManager.IsEmailTakenAsync(request.Request.Email, cancellationToken);
        if (isTaken) return Results.BadRequest("Email is already taken.");

        return Result.Success();
    }
}