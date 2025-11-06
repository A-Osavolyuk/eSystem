using eSecurity.Common.DTOs;
using eSecurity.Security.Identity.User;
using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Features.Users.Queries;

public record GetUserLinkedAccountDataQuery(Guid UserId) : IRequest<Result>;

public class GetUserLinkedAccountDataQueryHandler(
    IUserManager userManager) : IRequestHandler<GetUserLinkedAccountDataQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserLinkedAccountDataQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
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

        return Result.Success(response);
    }
}