using eSecurity.Security.Identity.Privacy;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;

namespace eSecurity.Features.Users.Commands;

public record RemovePersonalDataCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
}

public class RemovePersonalDataCommandHandler(
    IUserManager userManager,
    IPersonalDataManager personalDataManager) : IRequestHandler<RemovePersonalDataCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPersonalDataManager personalDataManager = personalDataManager;

    public async Task<Result> Handle(RemovePersonalDataCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        if (user.PersonalData is null) return Results.BadRequest("Personal data was not provided.");

        var result = await personalDataManager.DeleteAsync(user.PersonalData, cancellationToken);
        return result;
    }
}