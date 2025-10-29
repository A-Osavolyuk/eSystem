using eSystem.Auth.Api.Security.Authorization.Access;
using eSystem.Auth.Api.Security.Authorization.OAuth;
using eSystem.Auth.Api.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Authorization.Access;

namespace eSystem.Auth.Api.Features.LinkedAccounts.Commands;

public record DisconnectLinkedAccountCommand(DisconnectLinkedAccountRequest Request) : IRequest<Result>;

public class DisconnectLinkedAccountCommandHandler(
    IUserManager userManager,
    ILinkedAccountManager providerManager,
    IVerificationManager verificationManager) : IRequestHandler<DisconnectLinkedAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ILinkedAccountManager providerManager = providerManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(DisconnectLinkedAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var verificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.LinkedAccount, ActionType.Disconnect, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;
        
        var linkedAccount = user.GetLinkedAccount(request.Request.Type);;
        if (linkedAccount is null) return Results.NotFound("Cannot find user linked account.");

        var result = await providerManager.RemoveAsync(linkedAccount, cancellationToken);
        return result;
    }
}