using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record VerifyPhoneNumberCommand(VerifyPhoneNumberRequest Request) : IRequest<Result>;

public sealed class VerifyPhoneNumberCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager) : IRequestHandler<VerifyPhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(VerifyPhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID ${request.Request.UserId}");

        var verificationResult = await verificationManager.VerifyAsync(user,
            CodeResource.PhoneNumber, CodeType.Verify, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var result = await userManager.VerifyPhoneNumberAsync(user, request.Request.PhoneNumber, cancellationToken);
        return result;
    }
}