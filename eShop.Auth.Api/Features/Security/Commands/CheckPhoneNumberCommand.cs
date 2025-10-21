using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record CheckPhoneNumberCommand(CheckPhoneNumberRequest Request) : IRequest<Result>;

public class CheckPhoneNumberCommandHandler(IUserManager userManager) : IRequestHandler<CheckPhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(CheckPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var isTaken = await userManager.IsPhoneNumberTakenAsync(request.Request.PhoneNumber, cancellationToken);
        if (isTaken) return Results.BadRequest("Phone number is already taken.");
        
        return Result.Success();
    }
}