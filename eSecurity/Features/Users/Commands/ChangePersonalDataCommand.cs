using eSecurity.Security.Identity.Privacy;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;

namespace eSecurity.Features.Users.Commands;

public sealed record ChangePersonalDataCommand(ChangePersonalDataRequest Request) : IRequest<Result>;

public sealed class UpdatePersonalDataCommandHandler(
    IUserManager userManager,
    IPersonalDataManager personalDataManager) : IRequestHandler<ChangePersonalDataCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPersonalDataManager personalDataManager = personalDataManager;

    public async Task<Result> Handle(ChangePersonalDataCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        if (user.PersonalData is null) return Results.NotFound(
            $"Cannot find personal data of user with ID {request.Request.UserId}");
        
        user.PersonalData.FirstName = request.Request.FirstName;
        user.PersonalData.LastName = request.Request.LastName;
        user.PersonalData.MiddleName = request.Request.MiddleName;
        user.PersonalData.Gender = request.Request.Gender;
        user.PersonalData.BirthDate = request.Request.BirthDate!.Value;
        user.PersonalData.UpdateDate = DateTimeOffset.UtcNow;
        
        var result = await personalDataManager.UpdateAsync(user.PersonalData, cancellationToken);

        return result;
    }
}