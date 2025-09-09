using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record ResetPhoneNumberCommand(ResetPhoneNumberRequest Request) : IRequest<Result>;

public class ResetPhoneNumberCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager,
    IdentityOptions identityOptions) : IRequestHandler<ResetPhoneNumberCommand, Result>
{
    private readonly IdentityOptions identityOptions = identityOptions;
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(ResetPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        
        if(!user.HasPhoneNumber()) return Results.BadRequest("User does not have a phone number.");
        
        var userCurrentPhoneNumber = user.PhoneNumbers.FirstOrDefault(x => x.Type is PhoneNumberType.Primary);
        if (userCurrentPhoneNumber is null) return Results.BadRequest("User's primary phone number is missing");
        
        if (identityOptions.Account.RequireUniquePhoneNumber)
        {
            var isTaken = await userManager.IsPhoneNumberTakenAsync(request.Request.NewPhoneNumber, cancellationToken);
            if (isTaken) return Results.BadRequest("This phone number is already taken");
        }
        
        var verificationResult = await verificationManager.VerifyAsync(user, 
            CodeResource.PhoneNumber, CodeType.Reset, cancellationToken);
        
        if (!verificationResult.Succeeded) return verificationResult;
        
        var newPhoneNumber = request.Request.NewPhoneNumber;
        
        var result = await userManager.ResetPhoneNumberAsync(user, 
            userCurrentPhoneNumber.PhoneNumber, newPhoneNumber, cancellationToken);
        
        return result;
    }
}