using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record AddRecoveryEmailCommand(AddRecoveryEmailRequest Request) : IRequest<Result>;

public class AddRecoveryEmailCommandHandler(
    IUserManager userManager,
    IdentityOptions identityOptions) : IRequestHandler<AddRecoveryEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(AddRecoveryEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");

        if (user.HasRecoveryEmail()) return Results.BadRequest("User already has a recovery email.");
        
        if (identityOptions.Account.RequireUniqueRecoveryEmail)
        {
            var isTaken = await userManager.IsEmailTakenAsync(request.Request.RecoveryEmail, cancellationToken);
            if (isTaken) return Results.BadRequest("This email address is already taken");
        }
        
        var result = await userManager.AddRecoveryEmailAsync(user, request.Request.RecoveryEmail, cancellationToken);
        return result;
    }
}