using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Server.Security.Authorization.OAuth;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.OAuth;

public record LoadOAuthSessionCommand(LoadOAuthSessionRequest Request) : IRequest<Result>;

public class LoadOAuthSessionCommandHandler(
    IUserManager userManager,
    ILinkedAccountManager linkedAccountManager,
    IOAuthSessionManager sessionManager) : IRequestHandler<LoadOAuthSessionCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;
    private readonly IOAuthSessionManager _sessionManager = sessionManager;

    public async Task<Result> Handle(LoadOAuthSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionManager.FindAsync(request.Request.SessionId, request.Request.Token, cancellationToken);
        if (session is null || session.ExpiredDate < DateTimeOffset.UtcNow) 
            return Results.NotFound("Session was not found or already expired.");

        if (session.LinkedAccount is null) return Results.InternalServerError("Invalid session.");
        
        var user = await _userManager.FindByIdAsync(session.LinkedAccount.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {session.LinkedAccount.UserId}");

        var linkedAccount = await _linkedAccountManager.GetAsync(user, session.LinkedAccount.Type, cancellationToken);
        if (linkedAccount is null) return Results.NotFound("Linked account not found");
        
        var response = new LoadOAuthSessionResponse()
        {
            UserId = user.Id,
            SignType = session.SignType,
            LinkedAccount = linkedAccount.Type.ToString(),
        };

        return Results.Ok(response);
    }
}