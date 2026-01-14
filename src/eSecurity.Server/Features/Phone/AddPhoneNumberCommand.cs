using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.Phone;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Phone;

public record AddPhoneNumberCommand(AddPhoneNumberRequest Request) : IRequest<Result>;

public class AddPhoneNumberCommandHandler(
    IUserManager userManager,
    IPhoneManager phoneManager,
    IOptions<AccountOptions> options) : IRequestHandler<AddPhoneNumberCommand, Result>
{
    private readonly AccountOptions _options = options.Value;
    private readonly IUserManager _userManager = userManager;
    private readonly IPhoneManager _phoneManager = phoneManager;

    public async Task<Result> Handle(AddPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var phoneNumbers = await _phoneManager.GetAllAsync(user, cancellationToken);
        if (phoneNumbers.Count(x => x.Type is PhoneNumberType.Primary)
            >= _options.PrimaryPhoneNumberMaxCount && request.Request.Type is PhoneNumberType.Primary)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.Common.InvalidPhone,
                Description = "User already has a primary phone number."
            });
        }

        if (phoneNumbers.Count(x => x.Type is PhoneNumberType.Recovery)
            >= _options.RecoveryPhoneNumberMaxCount && request.Request.Type is PhoneNumberType.Recovery)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.Common.InvalidPhone,
                Description = "User already has a recovery phone number."
            });
        }

        if (phoneNumbers.Count(x => x.Type is PhoneNumberType.Secondary)
            >= _options.SecondaryPhoneNumberMaxCount && request.Request.Type is PhoneNumberType.Secondary)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.Common.InvalidPhone,
                Description = "User already has maximum count of secondary phone numbers."
            });
        }

        if (_options.RequireUniquePhoneNumber)
        {
            var isTaken = await _phoneManager.IsTakenAsync(request.Request.PhoneNumber, cancellationToken);
            if (isTaken)
            {
                return Results.BadRequest(new Error()
                {
                    Code = ErrorTypes.Common.PhoneTaken,
                    Description = "This phone number is already taken"
                });
            }
        }

        var result = await _phoneManager.AddAsync(user, request.Request.PhoneNumber,
            request.Request.Type, cancellationToken);

        if (!result.Succeeded) return result;

        return Results.Ok();
    }
}