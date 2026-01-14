using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.Phone;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Users.Queries;

public sealed record GetUserQuery(Guid UserId) : IRequest<Result>;

public sealed class GetUserQueryHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    IPhoneManager phoneManager) : IRequestHandler<GetUserQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly IPhoneManager _phoneManager = phoneManager;

    public async Task<Result> Handle(GetUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var email = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);

        var response = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = email?.Email
        };
        
        return Results.Ok(response);
    }
}