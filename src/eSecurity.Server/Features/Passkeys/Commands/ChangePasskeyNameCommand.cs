using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Passkeys.Commands;

public record ChangePasskeyNameCommand(ChangePasskeyNameRequest Request) : IRequest<Result>;

public class ChangePasskeyNameCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<ChangePasskeyNameCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly HttpContext _httpContext= httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(ChangePasskeyNameCommand request, CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid request"
            });
        }
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "User not found"
            });
        }

        if (!await _passkeyManager.HasAsync(user, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "User does not have any passkeys"
            });
        }

        var passkey = await _passkeyManager.FindByIdAsync(request.Request.PasskeyId, cancellationToken);
        if (passkey is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "Passkey not found"
            });
        }

        passkey.DisplayName = request.Request.DisplayName;

        var result = await _passkeyManager.UpdateAsync(passkey, cancellationToken);
        return result;
    }
}