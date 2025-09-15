using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record UnauthorizeCommand(UnauthorizeRequest Request) : IRequest<Result>;

public class UnauthorizeCommandHandler(
    IUserManager userManager,
    TokenHandler tokenHandler) : IRequestHandler<UnauthorizeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly TokenHandler tokenHandler = tokenHandler;

    public async Task<Result> Handle(UnauthorizeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        tokenHandler.Remove();

        return Result.Success();
    }
}