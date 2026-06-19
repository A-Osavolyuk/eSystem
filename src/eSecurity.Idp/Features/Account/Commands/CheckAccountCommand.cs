using eSecurity.Idp.Security.Authentication.Lockout;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Requests;
using eSecurity.Core.Responses;
using eSecurity.Core.Security.Identity;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Account.Commands;

public record CheckAccountCommand(CheckAccountRequest Request) : IRequest<Result>;

public class CheckAccountCommandHandler(
    IUserQueryService userQueryService,
    ILockoutManager lockoutManager) : IRequestHandler<CheckAccountCommand, Result>
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly ILockoutManager _lockoutManager = lockoutManager;

    public async Task<Result> Handle(CheckAccountCommand request, CancellationToken cancellationToken)
    {
        CheckAccountResponse? response;

        var user = await _userQueryService.GetByLoginAsync(request.Request.Login, cancellationToken);
        if (user is null)
        {
            response = new CheckAccountResponse { Exists = false };
            return Results.Success(SuccessCodes.Ok, response);
        }

        var lockoutState = await _lockoutManager.GetAsync(user, cancellationToken);
        if (lockoutState is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.InvalidLockoutState,
                Description = "Invalid state"
            });
        }

        response = new CheckAccountResponse
        {
            Exists = true
        };

        return Results.Success(SuccessCodes.Ok, response);
    }
}