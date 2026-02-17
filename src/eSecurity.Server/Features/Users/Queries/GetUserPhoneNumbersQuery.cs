using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Identity.Phone;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserPhoneNumbersQuery(string Subject) : IRequest<Result>;

public class GetUserPhoneNumbersQueryHandler(
    IUserManager userManager,
    IPhoneManager phoneManager) : IRequestHandler<GetUserPhoneNumbersQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPhoneManager _phoneManager = phoneManager;

    public async Task<Result> Handle(GetUserPhoneNumbersQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindBySubjectAsync(request.Subject, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var phoneNumbers = await _phoneManager.GetAllAsync(user, cancellationToken);
        var response = phoneNumbers.Select(phoneNumber => new UserPhoneNumberDto
        {
            Id = phoneNumber.Id,
            PhoneNumber = phoneNumber.PhoneNumber,
            Type = phoneNumber.Type,
            IsVerified = phoneNumber.IsVerified,
            VerifiedDate = phoneNumber.VerifiedDate,
            UpdateDate = phoneNumber.UpdateDate
        });
        
        return Results.Ok(response);
    }
}