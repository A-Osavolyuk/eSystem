using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record AuthorizeCommand(AuthorizeRequest Request) : IRequest<Result>;

public class AuthorizeCommandHandler(
    IUserManager userManager,
    ITokenManager tokenManager,
    TokenHandler tokenHandler) : IRequestHandler<AuthorizeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly TokenHandler tokenHandler = tokenHandler;

    public async Task<Result> Handle(AuthorizeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var token = await tokenManager.FindAsync(user, cancellationToken);
        if (token is null) return Results.NotFound($"User doesn't have refresh token.");
        
        tokenHandler.Set(token.Token, token.ExpireDate.DateTime);

        return Result.Success();
    }
}