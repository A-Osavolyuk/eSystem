using eSystem.Auth.Api.Security.Identity.User;
using eSystem.Core.DTOs;

namespace eSystem.Auth.Api.Features.Users.Queries;

public record GetUserPhoneNumbersQuery(Guid UserId) : IRequest<Result>;

public class GetUserPhoneNumbersQueryHandler(IUserManager userManager) : IRequestHandler<GetUserPhoneNumbersQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserPhoneNumbersQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var response = user.PhoneNumbers.Select(phoneNumber => new UserPhoneNumberDto()
        {
            Id = phoneNumber.Id,
            PhoneNumber = phoneNumber.PhoneNumber,
            Type = phoneNumber.Type,
            IsVerified = phoneNumber.IsVerified,
            VerifiedDate = phoneNumber.VerifiedDate,
            UpdateDate = phoneNumber.UpdateDate
        });
        
        return Result.Success(response);
    }
}