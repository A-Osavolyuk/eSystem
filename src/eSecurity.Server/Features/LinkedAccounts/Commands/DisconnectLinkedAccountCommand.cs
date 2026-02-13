using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Results;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Features.LinkedAccounts.Commands;

public record DisconnectLinkedAccountCommand(DisconnectLinkedAccountRequest Request) : IRequest<Result>;

public class DisconnectLinkedAccountCommandHandler(
    IUserManager userManager,
    ILinkedAccountManager linkedAccountManager,
    IVerificationManager verificationManager) : IRequestHandler<DisconnectLinkedAccountCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;
    private readonly IVerificationManager _verificationManager = verificationManager;

    public async Task<Result> Handle(DisconnectLinkedAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found");

        var verificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.LinkedAccount, ActionType.Disconnect, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var linkedAccount = await _linkedAccountManager.GetAsync(user, request.Request.Type, cancellationToken);
        if (linkedAccount is null) return Results.NotFound("Linked account not found.");

        var result = await _linkedAccountManager.RemoveAsync(linkedAccount, cancellationToken);
        return result;
    }
}