using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Authentication.SignIn.Session;

namespace eSecurity.Server.Features.Account.Queries;

public record GetSignInSessionQuery(Guid Sid) : IRequest<Result>;

public class GetSignInSessionQueryHandler(
    ISignInSessionManager signInSessionManager) : IRequestHandler<GetSignInSessionQuery, Result>
{
    private readonly ISignInSessionManager _signInSessionManager = signInSessionManager;

    public async Task<Result> Handle(GetSignInSessionQuery request, CancellationToken cancellationToken)
    {
        var session = await _signInSessionManager.FindByIdAsync(request.Sid, cancellationToken);
        if (session is null || !session.IsActive)
        {
            return Results.BadRequest("Invalid sign-in session.");
        }

        var response = new SignInSessionDto()
        {
            Id = session.Id,
            UserId = session.UserId
        };
        
        return Results.Ok(response);
    }
}