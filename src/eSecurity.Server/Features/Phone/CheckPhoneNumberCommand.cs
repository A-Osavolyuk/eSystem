using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Identity.Phone;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Features.Phone;

public record CheckPhoneNumberCommand(CheckPhoneNumberRequest Request) : IRequest<Result>;

public class CheckPhoneNumberCommandHandler(
    IUserManager userManager,
    IPhoneManager phoneManager) : IRequestHandler<CheckPhoneNumberCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPhoneManager _phoneManager = phoneManager;

    public async Task<Result> Handle(CheckPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var isTaken = await _phoneManager.IsTakenAsync(request.Request.PhoneNumber, cancellationToken);
        if (isTaken)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.PhoneTaken,
                Description = "Phone number is already taken."
            });
        }
        
        return Results.Ok();
    }
}