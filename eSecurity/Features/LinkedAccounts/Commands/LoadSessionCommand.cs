using eSecurity.Common.Responses;
using eSecurity.Security.Authorization.OAuth;
using eSecurity.Security.Identity.User;

namespace eSecurity.Features.LinkedAccounts.Commands;

public class LoadSessionCommand() : IRequest<Result>
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
}

public class LoadOAuthSessionCommandHandler(
    IUserManager userManager,
    IOAuthSessionManager sessionManager) : IRequestHandler<LoadSessionCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IOAuthSessionManager sessionManager = sessionManager;

    public async Task<Result> Handle(LoadSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await sessionManager.FindAsync(request.Id, request.Token, cancellationToken);
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