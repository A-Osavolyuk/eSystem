using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Identity.Phone;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserPrimaryPhoneNumberQuery(Guid UserId) : IRequest<Result>;

public class GetUserPrimaryPhoneNumberQueryHandler(
    IUserManager userManager,
    IPhoneManager phoneManager) : IRequestHandler<GetUserPrimaryPhoneNumberQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPhoneManager _phoneManager = phoneManager;

    public async Task<Result> Handle(GetUserPrimaryPhoneNumberQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID: {request.UserId}.");
        
        var phoneNumber = await _phoneManager.FindByTypeAsync(user, PhoneNumberType.Primary, cancellationToken);
        if (phoneNumber is null) return Results.BadRequest("User does not have a primary phone number.");

        var response = new UserPhoneNumberDto()
        {
            Id = phoneNumber.Id,
            PhoneNumber = phoneNumber.PhoneNumber,
            IsVerified = phoneNumber.IsVerified,
            Type = phoneNumber.Type,
            UpdateDate = phoneNumber.UpdateDate,
            VerifiedDate = phoneNumber.VerifiedDate
        };
        
        return Results.Ok(response);
    }
}