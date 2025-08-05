using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record LoadOAuthSessionCommand(LoadOAuthSessionRequest Request) : IRequest<Result>;

public class LoadOAuthSessionCommandHandler(
    IUserManager userManager,
    IOAuthSessionManager sessionManager,
    ITokenManager tokenManager) : IRequestHandler<LoadOAuthSessionCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IOAuthSessionManager sessionManager = sessionManager;
    private readonly ITokenManager tokenManager = tokenManager;

    public async Task<Result> Handle(LoadOAuthSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await sessionManager.FindAsync(request.Request.SessionId, cancellationToken);

        if (session is null)
        {
            return Results.NotFound("Cannot find session");
        }

        if (!session.IsSucceeded)
        {
            return Results.InternalServerError(session.ErrorMessage!);
        }

        if (!session.UserId.HasValue)
        {
            return Results.InternalServerError("User was not provided with session data");
        }
        
        var user = await userManager.FindByIdAsync(session.UserId.Value, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {session.UserId}");
        }
        
        var accessToken = await tokenManager.GenerateAsync(user, TokenType.Access, cancellationToken);
        var refreshToken = await tokenManager.GenerateAsync(user, TokenType.Refresh, cancellationToken);

        var response = new LoadOAuthSessionResponse()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            SignType = session.SignType,
            Provider = session.Provider,
        };

        return Result.Success(response);
    }
}