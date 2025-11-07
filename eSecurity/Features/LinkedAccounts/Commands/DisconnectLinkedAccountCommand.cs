using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Authorization.OAuth;
using eSecurity.Security.Identity.User;

namespace eSecurity.Features.LinkedAccounts.Commands;

public class DisconnectLinkedAccountCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
    public LinkedAccountType Type { get; set; }
}

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
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var verificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.LinkedAccount, ActionType.Disconnect, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;
        
        var linkedAccount = user.GetLinkedAccount(request.Type);;
        if (linkedAccount is null) return Results.NotFound("Cannot find user linked account.");

        var result = await providerManager.RemoveAsync(linkedAccount, cancellationToken);
        return result;
    }
}