using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserPrimaryEmailQuery(Guid UserId) : IRequest<Result>;

public class GetUserPrimaryEmailQueryHandler(IUserManager userManager)
    : IRequestHandler<GetUserPrimaryEmailQuery, Result>
{
    private readonly IUserManager _userManager = userManager;

    public async Task<Result> Handle(GetUserPrimaryEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID: {request.UserId}.");

        var userPrimaryEmail = user.GetEmail(EmailType.Primary);
        if (userPrimaryEmail is null) return Results.BadRequest("User does not have a primary email.");

        var response = new UserEmailDto()
        {
            Id = userPrimaryEmail.Id,
            Type = userPrimaryEmail.Type,
            Email = userPrimaryEmail.Email,
            NormalizedEmail = userPrimaryEmail.NormalizedEmail,
            IsVerified = userPrimaryEmail.IsVerified,
            UpdateDate = userPrimaryEmail.UpdateDate,
            VerifiedDate = userPrimaryEmail.VerifiedDate
        };

        return Result.Success(response);
    }
}