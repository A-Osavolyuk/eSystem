using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record RemoveEmailCommand(RemoveEmailRequest Request) : IRequest<Result>;

public class RemoveEmailCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager) : IRequestHandler<RemoveEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(RemoveEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var userEmail = user.Emails.FirstOrDefault(x => x.Email == request.Request.Email);
        if (userEmail is null) return Results.NotFound($"User doesn't have this email {request.Request.Email}.");

        if (userEmail.Type == EmailType.Primary)
        {
            if (user.CountPasskeys() == 0)
            {
                return Results.BadRequest(
                    "Cannot remove the primary email, because it is the only authentication method");
            }

            if (user.LinkedAccounts.Count >= 1)
            {
                return Results.BadRequest(
                    "Cannot remove the primary email, because there are one or more linked accounts");
            }
        }

        var verificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.Email, ActionType.Remove, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;
        
        var result = await userManager.RemoveEmailAsync(user, request.Request.Email, cancellationToken);
        return result;
    }
}