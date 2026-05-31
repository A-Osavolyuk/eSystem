using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.DTOs;
using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Idp.Security.Authorization.LinkedAccount;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Features.Users.Queries;

public record GetUserLinkedAccountDataQuery : IRequest<Result>;

public class GetUserLinkedAccountDataQueryHandler(
    IUserManager userManager,
    ILinkedAccountManager linkedAccountManager) : IRequestHandler<GetUserLinkedAccountDataQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;

    public async Task<Result> Handle(GetUserLinkedAccountDataQuery request, CancellationToken cancellationToken)
    {
        var userResult = await _userManager.GetUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        if (!userResult.TryGetValue(out var user))
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
            });
        }

        var linkedAccounts = await _linkedAccountManager.GetAllAsync(user, cancellationToken);
        var response = new UserLinkedAccountData
        {
            GoogleConnected = linkedAccounts.Any(x => x.Type == LinkedAccountType.Google),
            MicrosoftConnected = linkedAccounts.Any(x => x.Type == LinkedAccountType.Microsoft),
            FacebookConnected = linkedAccounts.Any(x => x.Type == LinkedAccountType.Facebook),
            XConnected = linkedAccounts.Any(x => x.Type == LinkedAccountType.X),
            LinkedAccounts = linkedAccounts.Select(x => new UserLinkedAccountDto
            {
                Id = x.Id,
                Type = x.Type,
                LinkedAt = x.CreatedAt,
            }).ToList()
        };

        return Results.Success(SuccessCodes.Ok, response);
    }
}