using eSecurity.Common.DTOs;
using eSecurity.Security.Identity.User;
using eSystem.Core.Security.Identity.Email;
using eSystem.Core.Security.Identity.PhoneNumber;

namespace eSecurity.Features.Users.Queries;

public sealed record GetUserQuery(Guid UserId) : IRequest<Result>;

public sealed class GetUserQueryHandler(
    IUserManager userManager) : IRequestHandler<GetUserQuery, Result>
{
    private readonly IUserManager userManager = userManager;
    
    public async Task<Result> Handle(GetUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var primaryEmail = user.GetEmail(EmailType.Primary);
        var primaryPhoneNumber = user.GetPhoneNumber(PhoneNumberType.Primary);

        var response = new UserDto
        {
            Id = user.Id,
            Email = primaryEmail?.Email,
            EmailConfirmed = primaryEmail?.IsVerified,
            EmailChangeDate = primaryEmail?.UpdateDate,
            EmailConfirmationDate = primaryEmail?.VerifiedDate,
            PhoneNumber = primaryPhoneNumber?.PhoneNumber,
            PhoneNumberConfirmed = primaryPhoneNumber?.IsVerified,
            PhoneNumberChangeDate = primaryPhoneNumber?.UpdateDate,
            PhoneNumberConfirmationDate = primaryPhoneNumber?.VerifiedDate,
            Username = user.Username,
            UserNameChangeDate = user.UsernameChangeDate,
        };
        
        return Result.Success(response);
    }
}