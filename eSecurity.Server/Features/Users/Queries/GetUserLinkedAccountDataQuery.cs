using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserLinkedAccountDataQuery(Guid UserId) : IRequest<Result>;

public class GetUserLinkedAccountDataQueryHandler(
    IUserManager userManager) : IRequestHandler<GetUserLinkedAccountDataQuery, Result>
{
    private readonly IUserManager _userManager = userManager;

    public async Task<Result> Handle(GetUserLinkedAccountDataQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var response = new UserLinkedAccountData()
        {
            GoogleConnected = user.HasLinkedAccount(LinkedAccountType.Google),
            MicrosoftConnected = user.HasLinkedAccount(LinkedAccountType.Microsoft),
            FacebookConnected = user.HasLinkedAccount(LinkedAccountType.Facebook),
            XConnected = user.HasLinkedAccount(LinkedAccountType.X),
            LinkedAccounts = user.LinkedAccounts.Select(x => new UserLinkedAccountDto()
            {
                Id = x.Id,
                Type = x.Type,
                LinkedDate = x.CreateDate,
            }).ToList()
        };

        return Results.Ok(response);
    }
}