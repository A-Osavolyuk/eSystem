using eSecurity.Core.Requests;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Users;

public record CheckUsernameCommand(CheckUsernameRequest Request) : IRequest<Result>;

public class CheckUsernameCommandHandler(
    IUserQueryService userQueryService) : IRequestHandler<CheckUsernameCommand, Result>
{
    private readonly IUserQueryService _userQueryService = userQueryService;

    public async Task<Result> Handle(CheckUsernameCommand request, CancellationToken cancellationToken)
    {
        if (await _userQueryService.ExistsAsync(request.Request.Username, cancellationToken))
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