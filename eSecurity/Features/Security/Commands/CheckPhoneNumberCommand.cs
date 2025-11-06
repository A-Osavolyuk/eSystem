using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;

namespace eSecurity.Features.Security.Commands;

public record CheckPhoneNumberCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
    public required string PhoneNumber { get; set; }
}

public class CheckPhoneNumberCommandHandler(IUserManager userManager) : IRequestHandler<CheckPhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(CheckPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var isTaken = await userManager.IsPhoneNumberTakenAsync(request.PhoneNumber, cancellationToken);
        if (isTaken) return Results.BadRequest("Phone number is already taken.");
        
        return Result.Success();
    }
}