using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

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

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }
        
        var codes = await recoverManager.FindAsync(user, cancellationToken);
        var result = codes.Select(Mapper.Map).ToList();
        
        return Result.Success(result);
    }
}