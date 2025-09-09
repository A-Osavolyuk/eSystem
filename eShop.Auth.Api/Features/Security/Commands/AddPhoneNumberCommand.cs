using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record AddPhoneNumberCommand(AddPhoneNumberRequest Request) : IRequest<Result>;

public class AddPhoneNumberCommandHandler(
    IUserManager userManager,
    IdentityOptions identityOptions) : IRequestHandler<AddPhoneNumberCommand, Result>
{
    private readonly IdentityOptions identityOptions = identityOptions;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(AddPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");

        if (user.PhoneNumbers.Count(x => x.Type is PhoneNumberType.Primary)
            >= identityOptions.Account.PrimaryPhoneNumberMaxCount && request.Request.Type is PhoneNumberType.Primary)
            return Results.BadRequest("User already has a primary phone number.");

        if (user.PhoneNumbers.Count(x => x.Type is PhoneNumberType.Secondary)
            >= identityOptions.Account.SecondaryPhoneNumberMaxCount && request.Request.Type is PhoneNumberType.Secondary)
            return Results.BadRequest("User already has maximum count of secondary phone numbers.");

        if (identityOptions.Account.RequireUniquePhoneNumber)
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