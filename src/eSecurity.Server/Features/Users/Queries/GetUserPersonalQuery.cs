using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Identity.Privacy;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserPersonalQuery(Guid UserId) : IRequest<Result>;

public class GetUserPersonalQueryHandler(
    IUserManager userManager,
    IPersonalDataManager personalDataManager) : IRequestHandler<GetUserPersonalQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPersonalDataManager _personalDataManager = personalDataManager;

    public async Task<Result> Handle(GetUserPersonalQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var personalData = await _personalDataManager.GetAsync(user, cancellationToken);
        if (personalData is null) return Results.NotFound("Personal data not found.");
        
        var response = new UserPersonalDto()
        {
            UserId = user.Id,
            FirstName = personalData.FirstName,
            LastName = personalData.LastName,
            MiddleName = personalData.MiddleName,
            Gender = personalData.Gender,
            BirthDate = personalData.BirthDate,
            UpdateDate = personalData.UpdateDate
        };
        
        return Results.Ok(response);
    }
}