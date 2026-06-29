using eSecurity.Core.DTOs;
using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Idp.Security.Authorization.LinkedAccount;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Users;

public record GetUserLinkedAccountDataQuery : IRequest<Result>;

public class GetUserLinkedAccountDataQueryHandler(
    ICurrentUserAccessor currentUserAccessor,
    ILinkedAccountQueryService linkedAccountQueryService) : IRequestHandler<GetUserLinkedAccountDataQuery, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly ILinkedAccountQueryService _linkedAccountQueryService = linkedAccountQueryService;

    public async Task<Result> Handle(GetUserLinkedAccountDataQuery request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var linkedAccounts = await _linkedAccountQueryService.ListByUserAsync(user.Id, cancellationToken);
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