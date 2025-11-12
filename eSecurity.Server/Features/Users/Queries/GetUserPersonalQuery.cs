using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserPersonalQuery(Guid UserId) : IRequest<Result>;

public class GetUserPersonalQueryHandler(IUserManager userManager) : IRequestHandler<GetUserPersonalQuery, Result>
{
    private readonly IUserManager _userManager = userManager;

    public async Task<Result> Handle(GetUserPersonalQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        if (user.PersonalData is null) return Results.NotFound(
            $"Cannot find personal data of user with ID {request.UserId}.");
        
        var response = new UserPersonalDto()
        {
            UserId = user.Id,
            FirstName = user.PersonalData.FirstName,
            LastName = user.PersonalData.LastName,
            MiddleName = user.PersonalData.MiddleName,
            Gender = user.PersonalData.Gender,
            BirthDate = user.PersonalData.BirthDate,
            UpdateDate = user.PersonalData.UpdateDate
        };
        
        return Result.Success(response);
    }
}