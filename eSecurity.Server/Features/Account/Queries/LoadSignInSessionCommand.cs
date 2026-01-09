using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Authentication.SignIn.Session;
using eSecurity.Server.Security.Authentication.SignIn.Session;

namespace eSecurity.Server.Features.Account.Queries;

public record LoadSignInSessionCommand(Guid Sid) : IRequest<Result>;

public class LoadSignInSessionCommandHandler(
    ISignInSessionManager signInSessionManager) : IRequestHandler<LoadSignInSessionCommand, Result>
{
    private readonly ISignInSessionManager _signInSessionManager = signInSessionManager;

    public async Task<Result> Handle(LoadSignInSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _signInSessionManager.FindByIdAsync(request.Sid, cancellationToken);
        if (session is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.InvalidSession,
                Description = "Session not found."
            });
        }
        
        if (!session.IsActive)
        {
            if (session.Status != SignInStatus.Expired)
            {
                session.Status = SignInStatus.Expired;
            
                var sessionResult = await _signInSessionManager.UpdateAsync(session, cancellationToken);
                if (!sessionResult.Succeeded) return sessionResult;
            }
            
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.InvalidSession,
                Description = "Session is expired."
            });
        }
        
        if (session.Status == SignInStatus.Cancelled)
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.InvalidSession,
                Description = "Session is cancelled."
            });
        }
        
        var response = new SignInSessionDto()
        {
            Id = session.Id,
            UserId = session.UserId,
            Provider = session.Provider,
            OAuthFlow = session.OAuthFlow,
            NextStep = session.RequiredSteps.Except([..session.CompletedSteps]).Any() 
                ? session.RequiredSteps.Except([..session.CompletedSteps]).First() 
                : SignInStep.Complete
        };
        
        return Results.Ok(response);
    }
}