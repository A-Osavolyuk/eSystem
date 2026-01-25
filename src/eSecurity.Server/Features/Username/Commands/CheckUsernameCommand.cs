using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Identity.User.Username;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Username.Commands;

public record CheckUsernameCommand(CheckUsernameRequest Request) : IRequest<Result>;

public class CheckUsernameCommandHandler(
    IUsernameManager usernameManager) : IRequestHandler<CheckUsernameCommand, Result>
{
    private readonly IUsernameManager _usernameManager = usernameManager;
    
    public async Task<Result> Handle(CheckUsernameCommand request, CancellationToken cancellationToken)
    {
        if (await _usernameManager.IsTakenAsync(request.Request.Username, cancellationToken))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.UsernameTaken,
                Description = "The username is already taken."
            });
        }
        
        return Results.Ok();
    }
}