using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserLinkedAccountDataQuery(Guid UserId) : IRequest<Result>;

public class GetUserLinkedAccountDataQueryHandler(
    IUserManager userManager,
    ILinkedAccountManager linkedAccountManager) : IRequestHandler<GetUserLinkedAccountDataQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;

    public async Task<Result> Handle(GetUserLinkedAccountDataQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var linkedAccounts = await _linkedAccountManager.GetAllAsync(user, cancellationToken);
        var response = new UserLinkedAccountData()
        {
            GoogleConnected = linkedAccounts.Any(x => x.Type == LinkedAccountType.Google),
            MicrosoftConnected = linkedAccounts.Any(x => x.Type == LinkedAccountType.Microsoft),
            FacebookConnected = linkedAccounts.Any(x => x.Type == LinkedAccountType.Facebook),
            XConnected = linkedAccounts.Any(x => x.Type == LinkedAccountType.X),
            LinkedAccounts = linkedAccounts.Select(x => new UserLinkedAccountDto()
            {
                Id = x.Id,
                Type = x.Type,
                LinkedDate = x.CreateDate,
            }).ToList()
        };

        return Results.Ok(response);
    }
}