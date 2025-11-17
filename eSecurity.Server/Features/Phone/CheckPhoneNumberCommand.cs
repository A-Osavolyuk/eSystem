using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Phone;

public record CheckPhoneNumberCommand(CheckPhoneNumberRequest Request) : IRequest<Result>;

public class CheckPhoneNumberCommandHandler(IUserManager userManager) : IRequestHandler<CheckPhoneNumberCommand, Result>
{
    private readonly IUserManager _userManager = userManager;

    public async Task<Result> Handle(CheckPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var isTaken = await _userManager.IsPhoneNumberTakenAsync(request.Request.PhoneNumber, cancellationToken);
        if (isTaken) return Results.BadRequest("Phone number is already taken.");
        
        return Results.Ok();
    }
}