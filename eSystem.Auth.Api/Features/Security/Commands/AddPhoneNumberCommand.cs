using eSystem.Auth.Api.Interfaces;
using eSystem.Auth.Api.Security.Identity.Options;
using eSystem.Domain.Common.Results;
using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Features.Security.Commands;

public record AddPhoneNumberCommand(AddPhoneNumberRequest Request) : IRequest<Result>;

public class AddPhoneNumberCommandHandler(
    IUserManager userManager,
    IOptions<AccountOptions> options) : IRequestHandler<AddPhoneNumberCommand, Result>
{
    private readonly AccountOptions options = options.Value;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(AddPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");

        if (user.PhoneNumbers.Count(x => x.Type is PhoneNumberType.Primary)
            >= options.PrimaryPhoneNumberMaxCount && request.Request.Type is PhoneNumberType.Primary)
            return Results.BadRequest("User already has a primary phone number.");
        
        if (user.PhoneNumbers.Count(x => x.Type is PhoneNumberType.Recovery)
            >= options.RecoveryPhoneNumberMaxCount && request.Request.Type is PhoneNumberType.Recovery)
            return Results.BadRequest("User already has a recovery phone number.");

        if (user.PhoneNumbers.Count(x => x.Type is PhoneNumberType.Secondary)
            >= options.SecondaryPhoneNumberMaxCount && request.Request.Type is PhoneNumberType.Secondary)
            return Results.BadRequest("User already has maximum count of secondary phone numbers.");

        if (options.RequireUniquePhoneNumber)
        {
            var isTaken = await userManager.IsPhoneNumberTakenAsync(request.Request.PhoneNumber, cancellationToken);
            if (isTaken) return Results.BadRequest("This phone number is already taken");
        }

        var result = await userManager.AddPhoneNumberAsync(user, request.Request.PhoneNumber,
            request.Request.Type, cancellationToken);

        if (!result.Succeeded) return result;

        return Result.Success();
    }
}