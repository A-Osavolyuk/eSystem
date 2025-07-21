using eShop.Domain.Requests;

namespace eShop.Auth.Api.Features.Security.Commands;

public record RollbackCommand(RollbackRequest Request) : IRequest<Result>;

public class RollbackCommandHandler(
    IRollbackManager rollbackManager,
    IUserManager userManager) : IRequestHandler<RollbackCommand, Result>
{
    private readonly IRollbackManager rollbackManager = rollbackManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(RollbackCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }
        
        var code = request.Request.Code;
        var field = request.Request.Field;

        var result = await rollbackManager.RollbackAsync(user, code, field, cancellationToken);

        return result;
    }
}