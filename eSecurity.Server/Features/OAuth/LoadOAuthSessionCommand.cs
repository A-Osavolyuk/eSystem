using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Server.Data;
using eSecurity.Server.Security.Authorization.OAuth;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.OAuth;

public record LoadOAuthSessionCommand(LoadOAuthSessionRequest Request) : IRequest<Result>;

public class LoadOAuthSessionCommandHandler(
    IUserManager userManager,
    IOAuthSessionManager sessionManager) : IRequestHandler<LoadOAuthSessionCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IOAuthSessionManager sessionManager = sessionManager;

    public async Task<Result> Handle(LoadOAuthSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await sessionManager.FindAsync(request.Request.SessionId, request.Request.Token, cancellationToken);
        if (session is null || session.ExpiredDate < DateTimeOffset.UtcNow) 
            return Results.NotFound("Session was not found or already expired.");

        if (session.LinkedAccount is null) return Results.InternalServerError("Invalid session.");
        
        var user = await userManager.FindByIdAsync(session.LinkedAccount.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {session.LinkedAccount.UserId}");

        var linkedAccount = user.GetLinkedAccount(session.LinkedAccount.Type)!;
        var response = new LoadOAuthSessionResponse()
        {
            UserId = user.Id,
            SignType = session.SignType,
            LinkedAccount = linkedAccount.Type.ToString(),
        };

        return Result.Success(response);
    }
}