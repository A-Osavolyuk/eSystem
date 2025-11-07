using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Identity.User;
using eSystem.Core.Security.Identity.Email;

namespace eSecurity.Features.Security.Commands;

public record ManageEmailCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
    public required EmailType Type { get; set; }
}

public class ManageEmailCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager) : IRequestHandler<ManageEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(ManageEmailCommand request, CancellationToken cancellationToken)
    {
        var type = request.Type;
        var email = request.Email;
        
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var verificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.Email, ActionType.Manage, cancellationToken);
        
        if (!verificationResult.Succeeded) return verificationResult;

        var result = await userManager.ManageEmailAsync(user, type, email, cancellationToken);
        return result;
    }
}