using eSecurity.Security.Identity.Options;
using eSecurity.Security.Identity.User;
using eSystem.Core.Security.Identity.PhoneNumber;

namespace eSecurity.Features.Security.Commands;

public record AddPhoneNumberCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
    public required string PhoneNumber { get; set; } = string.Empty;
    public required PhoneNumberType Type { get; set; }
}

public class AddPhoneNumberCommandHandler(
    IUserManager userManager,
    IOptions<AccountOptions> options) : IRequestHandler<AddPhoneNumberCommand, Result>
{
    private readonly AccountOptions options = options.Value;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(AddPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}");

        if (user.PhoneNumbers.Count(x => x.Type is PhoneNumberType.Primary)
            >= options.PrimaryPhoneNumberMaxCount && request.Type is PhoneNumberType.Primary)
            return Results.BadRequest("User already has a primary phone number.");
        
        if (user.PhoneNumbers.Count(x => x.Type is PhoneNumberType.Recovery)
            >= options.RecoveryPhoneNumberMaxCount && request.Type is PhoneNumberType.Recovery)
            return Results.BadRequest("User already has a recovery phone number.");

        if (user.PhoneNumbers.Count(x => x.Type is PhoneNumberType.Secondary)
            >= options.SecondaryPhoneNumberMaxCount && request.Type is PhoneNumberType.Secondary)
            return Results.BadRequest("User already has maximum count of secondary phone numbers.");

        if (options.RequireUniquePhoneNumber)
        {
            var isTaken = await userManager.IsPhoneNumberTakenAsync(request.PhoneNumber, cancellationToken);
            if (isTaken) return Results.BadRequest("This phone number is already taken");
        }

        var result = await userManager.AddPhoneNumberAsync(user, request.PhoneNumber,
            request.Type, cancellationToken);

        if (!result.Succeeded) return result;

        return Result.Success();
    }
}