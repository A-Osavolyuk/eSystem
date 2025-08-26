using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ResetPasswordCommand(ResetPasswordRequest Request) : IRequest<Result>;

public sealed class ResetPasswordCommandHandler(IUserManager userManager) : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var resetResult = await userManager.ResetPasswordAsync(user, request.Request.NewPassword, cancellationToken);
        
        return resetResult;
    }
}