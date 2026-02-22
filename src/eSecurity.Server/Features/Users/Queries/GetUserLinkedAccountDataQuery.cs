using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserLinkedAccountDataQuery : IRequest<Result>;

public class GetUserLinkedAccountDataQueryHandler(
    IUserManager userManager,
    ILinkedAccountManager linkedAccountManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetUserLinkedAccountDataQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GetUserLinkedAccountDataQuery request, CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.BadRequest("Invalid subject.");
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

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
                LinkedDate = x.CreateDate,
            }).ToList()
        };

        return Results.Ok(response);
    }
}