using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.LinkedAccounts.Commands;

public record DisallowLinkedAccountCommand(DisallowLinkedAccountRequest Request) : IRequest<Result>;

public class DisallowLinkedAccountCommandHandler(
    IUserManager userManager,
    ILinkedAccountManager providerManager,
    IVerificationManager verificationManager) : IRequestHandler<DisallowLinkedAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ILinkedAccountManager providerManager = providerManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(DisallowLinkedAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var verificationResult = await verificationManager.VerifyAsync(user, 
            PurposeType.LinkedAccount, ActionType.Disallow, cancellationToken);
        
        if(!verificationResult.Succeeded) return verificationResult;
        
        var linkedAccount = user.GetLinkedAccount(request.Request.Type);
        if (linkedAccount is null) return Results.NotFound("Cannot find user linked account.");

        var result = await providerManager.DisallowAsync(linkedAccount, cancellationToken);
        return result;
    }
}