using eSecurity.Security.Authentication.TwoFactor.Recovery;
using eSecurity.Security.Identity.User;

namespace eSecurity.Features.TwoFactor.Commands;

public record GenerateRecoveryCodesCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
}

public class GenerateRecoveryCodesCommandHandler(
    IRecoverManager recoverManager,
    IUserManager userManager) : IRequestHandler<GenerateRecoveryCodesCommand, Result>
{
    private readonly IRecoverManager recoverManager = recoverManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GenerateRecoveryCodesCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}");

        var codes = await recoverManager.GenerateAsync(user, cancellationToken);
        
        return Result.Success(codes);
    }
}