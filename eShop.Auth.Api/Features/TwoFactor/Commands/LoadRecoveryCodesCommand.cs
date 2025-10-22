using eShop.Domain.Common.Results;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

public record LoadRecoveryCodesCommand(LoadRecoveryCodesRequest Request) : IRequest<Result>;

public class LoadRecoveryCodesCommandHandler(
    IUserManager userManager,
    IRecoverManager recoverManager) : IRequestHandler<LoadRecoveryCodesCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IRecoverManager recoverManager = recoverManager;
    
    public async Task<Result> Handle(LoadRecoveryCodesCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var codes = recoverManager.Unprotect(user);
        return Result.Success(codes);
    }
}