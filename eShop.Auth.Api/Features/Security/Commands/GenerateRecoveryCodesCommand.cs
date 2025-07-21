using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

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

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        }

        var codes = await recoverManager.GenerateAsync(user, cancellationToken);

        return Result.Success(codes);
    }
}