using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record RecoverAccountCommand(RecoverAccountRequest Request) : IRequest<Result>;

public class RecoverAccountCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager) : IRequestHandler<RecoverAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(RecoverAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var userPrimaryEmail = user.Emails.FirstOrDefault(e => e.Type == EmailType.Primary);
        if (userPrimaryEmail is null) return Results.BadRequest("User does not have a primary email.");

        var userRecoveryEmail = user.Emails.FirstOrDefault(e => e.Type == EmailType.Recovery);
        if (userRecoveryEmail is null) return Results.BadRequest("User does not have a recovery email.");

        var verificationResult = await verificationManager.VerifyAsync(user,
            CodeResource.Account, CodeType.Recover, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;
        
        var emailResult = await userManager.AsPrimaryAsync(user, userRecoveryEmail, cancellationToken);
        return emailResult;
    }
}