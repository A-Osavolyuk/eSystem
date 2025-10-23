using eSystem.Auth.Api.Interfaces;
using eSystem.Domain.Common.Results;
using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Features.TwoFactor.Commands;

public record GenerateRecoveryCodesCommand(GenerateRecoveryCodesRequest Request) : IRequest<Result>;

public class GenerateRecoveryCodesCommandHandler(
    IRecoverManager recoverManager,
    IUserManager userManager) : IRequestHandler<GenerateRecoveryCodesCommand, Result>
{
    private readonly IRecoverManager recoverManager = recoverManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GenerateRecoveryCodesCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");

        var codes = await recoverManager.GenerateAsync(user, cancellationToken);
        
        return Result.Success(codes);
    }
}