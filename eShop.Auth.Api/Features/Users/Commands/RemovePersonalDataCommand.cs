using eShop.Domain.Common.Results;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Users.Commands;

public record RemovePersonalDataCommand(RemovePersonalDataRequest Request) : IRequest<Result>;

public class RemovePersonalDataCommandHandler(
    IUserManager userManager,
    IPersonalDataManager personalDataManager) : IRequestHandler<RemovePersonalDataCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPersonalDataManager personalDataManager = personalDataManager;

    public async Task<Result> Handle(RemovePersonalDataCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        if (user.PersonalData is null) return Results.BadRequest("Personal data was not provided.");

        var result = await personalDataManager.DeleteAsync(user.PersonalData, cancellationToken);
        return result;
    }
}