using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Email.Commands;

public record ManageEmailCommand(ManageEmailRequest Request) : IRequest<Result>;

public class ManageEmailCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager) : IRequestHandler<ManageEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(ManageEmailCommand request, CancellationToken cancellationToken)
    {
        var type = request.Request.Type;
        var email = request.Request.Email;
        
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var verificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.Email, ActionType.Manage, cancellationToken);
        
        if (!verificationResult.Succeeded) return verificationResult;

        var result = await userManager.ManageEmailAsync(user, type, email, cancellationToken);
        return result;
    }
}