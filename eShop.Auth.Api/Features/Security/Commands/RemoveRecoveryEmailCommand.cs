using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record RemoveRecoveryEmailCommand(RemoveRecoveryEmailRequest Request) : IRequest<Result>;

public class RemoveRecoveryEmailCommandHandler(IUserManager userManager) : IRequestHandler<RemoveRecoveryEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(RemoveRecoveryEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        
        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }
        
        var result = await userManager.RemoveRecoveryEmailAsync(user, cancellationToken);
        
        return result;
    }
}