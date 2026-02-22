using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives.Constants;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Password.Commands;

public sealed record ChangePasswordCommand(ChangePasswordRequest Request) : IRequest<Result>;

public sealed class ChangePasswordCommandHandler(
    IUserManager userManager,
    IPasswordManager passwordManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    async Task<Result> IRequestHandler<ChangePasswordCommand, Result>.Handle(ChangePasswordCommand request,
            CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.BadRequest("Invalid request");
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        if (!await _passwordManager.HasAsync(user, cancellationToken))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidPassword,
                Description = "User does not have a password."
            });
        }

        if (!await _passwordManager.CheckAsync(user, request.Request.CurrentPassword, cancellationToken))
        {
            return Results.BadRequest(new Error
            {
                Code = "Invalid password",
                Description = "Invalid password."
            });
        }
        
        var result = await _passwordManager.ChangeAsync(user, request.Request.NewPassword, cancellationToken);

        return result;
    }
}