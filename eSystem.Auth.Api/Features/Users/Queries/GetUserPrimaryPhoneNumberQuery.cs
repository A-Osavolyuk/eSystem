using eSystem.Domain.DTOs;

namespace eSystem.Auth.Api.Features.Users.Queries;

public record GetUserPrimaryPhoneNumberQuery(Guid UserId) : IRequest<Result>;

public class GetUserPrimaryPhoneNumberQueryHandler(
    IUserManager userManager) : IRequestHandler<GetUserPrimaryPhoneNumberQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserPrimaryPhoneNumberQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID: {request.UserId}.");
        
        var primaryPhoneNumber = user.GetPhoneNumber(PhoneNumberType.Primary);
        if (primaryPhoneNumber is null) return Results.BadRequest("User does not have a primary phone number.");

        var response = new UserPhoneNumberDto()
        {
            Id = primaryPhoneNumber.Id,
            PhoneNumber = primaryPhoneNumber.PhoneNumber,
            IsVerified = primaryPhoneNumber.IsVerified,
            Type = primaryPhoneNumber.Type,
            UpdateDate = primaryPhoneNumber.UpdateDate,
            VerifiedDate = primaryPhoneNumber.VerifiedDate
        };
        
        return Result.Success(response);
    }
}