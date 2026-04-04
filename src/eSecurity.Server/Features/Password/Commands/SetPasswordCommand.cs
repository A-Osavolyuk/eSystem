using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Password.Commands;

public sealed record SetPasswordCommand(SetPasswordRequest Request) : IRequest<Result>;

public sealed class SetPasswordCommandHandler(
    IUserManager userManager,
    IPasswordManager passwordManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<SetPasswordCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly HttpContext _httpContext= httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(SetPasswordCommand request, CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.BadRequest("Invalid request");
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        if (!await _passwordManager.HasAsync(user, cancellationToken))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorCode.InvalidPassword,
                Description = "User does not have a password."
            });
        }
        
        var result = await _passwordManager.ResetAsync(user, request.Request.Password, cancellationToken);
        return result;
    }
}