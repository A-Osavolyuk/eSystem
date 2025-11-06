using eSecurity.Security.Identity.User;

namespace eSecurity.Features.Security.Commands;

public record CheckEmailCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
}

public class CheckEmailCommandHandler(
    IUserManager userManager) : IRequestHandler<CheckEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(CheckEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var isTaken = await userManager.IsEmailTakenAsync(request.Email, cancellationToken);
        if (isTaken) return Results.BadRequest("Email is already taken.");

        return Result.Success();
    }
}