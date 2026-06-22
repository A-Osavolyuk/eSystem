using eSecurity.Core.Requests;
using eSecurity.Idp.Security.Identity.User.Username;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Users;

public record CheckUsernameCommand(CheckUsernameRequest Request) : IRequest<Result>;

public class CheckUsernameCommandHandler(
    IUsernameManager usernameManager) : IRequestHandler<CheckUsernameCommand, Result>
{
    private readonly IUsernameManager _usernameManager = usernameManager;
    
    public async Task<Result> Handle(CheckUsernameCommand request, CancellationToken cancellationToken)
    {
        if (await _usernameManager.IsTakenAsync(request.Request.Username, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.UsernameTaken,
                Description = "The username is already taken."
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}