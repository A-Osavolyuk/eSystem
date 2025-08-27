using eShop.Auth.Api.Messages.Sms;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ChangePhoneNumberCommand(ChangePhoneNumberRequest Request) : IRequest<Result>;

public sealed class RequestChangePhoneNumberCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager,
    IdentityOptions identityOptions) : IRequestHandler<ChangePhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(ChangePhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        if (identityOptions.Account.RequireUniquePhoneNumber)
        {
            var isTaken = await userManager.IsPhoneNumberTakenAsync(request.Request.NewPhoneNumber, cancellationToken);
            if (isTaken) return Results.BadRequest("This phone number is already taken");
        }

        var currentPhoneNumberVerificationResult = await verificationManager.VerifyAsync(user,
            CodeResource.PhoneNumber, CodeType.Current, cancellationToken);

        if (!currentPhoneNumberVerificationResult.Succeeded) return currentPhoneNumberVerificationResult;

        var newPhoneNumberVerificationResult = await verificationManager.VerifyAsync(user,
            CodeResource.PhoneNumber, CodeType.New, cancellationToken);

        if (!newPhoneNumberVerificationResult.Succeeded) return newPhoneNumberVerificationResult;

        var result = await userManager.ChangePhoneNumberAsync(user, request.Request.NewPhoneNumber, cancellationToken);
        return result;
    }
}