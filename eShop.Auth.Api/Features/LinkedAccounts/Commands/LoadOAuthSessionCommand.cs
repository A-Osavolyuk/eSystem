using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.LinkedAccounts.Commands;

public record LoadOAuthSessionCommand(LoadOAuthSessionRequest Request) : IRequest<Result>;

public class LoadOAuthSessionCommandHandler(
    IUserManager userManager,
    IOAuthSessionManager sessionManager,
    IOAuthProviderManager providerManager) : IRequestHandler<LoadOAuthSessionCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IOAuthSessionManager sessionManager = sessionManager;
    private readonly IOAuthProviderManager providerManager = providerManager;

    public async Task<Result> Handle(LoadOAuthSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await sessionManager.FindAsync(request.Request.Id, request.Request.Token, cancellationToken);

        if (session is null || session.ExpiredDate < DateTimeOffset.UtcNow)
        {
            return Results.NotFound("Session was not found or already expired");
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

        if (!session.ProviderId.HasValue)
        {
            return Results.InternalServerError("Provider was not provided with session data");
        }
        
        var provider = await providerManager.FindByIdAsync(session.ProviderId.Value, cancellationToken);
        
        if (provider is null)
        {
            return Results.NotFound($"Cannot find provider with ID {session.ProviderId}");
        }
        

        var response = new LoadOAuthSessionResponse()
        {
            UserId = user.Id,
            SignType = session.SignType,
            Provider = provider.Name,
        };

        return Result.Success(response);
    }
}