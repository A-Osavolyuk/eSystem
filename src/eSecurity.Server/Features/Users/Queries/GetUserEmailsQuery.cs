using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserEmailsQuery : IRequest<Result>;

public class GetUserEmailsQueryHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetUserEmailsQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GetUserEmailsQuery request, CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.BadRequest("Invalid subject.");
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");
        
        var emails = await _emailManager.GetAllAsync(user, cancellationToken);
        var response = emails.Select(email => new UserEmailDto
        {
            Id = email.Id,
            Email = email.Email,
            NormalizedEmail = email.NormalizedEmail,
            Type = email.Type,
            IsVerified = email.IsVerified,
            VerifiedDate = email.VerifiedDate,
            UpdateDate = email.UpdateDate
        });

        return Results.Ok(response);
    }
}