using System.Text.Json.Serialization;
using eSecurity.Core.Requests;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.TwoFactor;

public record PreferTwoFactorMethodCommand : IRequest<Result>
{
    [JsonPropertyName("preferred_method")]
    public TwoFactorMethod PreferredMethod { get; set; }
}

public class PreferMethodCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    ITwoFactorManager twoFactorManager) : IRequestHandler<PreferTwoFactorMethodCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;

    public async Task<Result> Handle(PreferTwoFactorMethodCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        if (!userResult.TryGetValue(out var user))
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
            });
        }

        var result = await _twoFactorManager.PreferAsync(user, request.PreferredMethod, cancellationToken);
        return result;
    }
}