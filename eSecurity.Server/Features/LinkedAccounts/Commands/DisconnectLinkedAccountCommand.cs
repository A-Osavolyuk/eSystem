using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Data;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.LinkedAccounts.Commands;

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