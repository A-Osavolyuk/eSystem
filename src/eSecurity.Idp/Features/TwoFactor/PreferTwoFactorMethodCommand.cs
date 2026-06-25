using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.TwoFactor;

public sealed class PreferTwoFactorMethodCommand : IRequest<Result>
{
    [JsonPropertyName("preferred_method")]
    public TwoFactorMethod PreferredMethod { get; set; }
}

public sealed class PreferMethodCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    ITwoFactorQueryService twoFactorQueryService,
    ITwoFactorCommandService twoFactorCommandService) : IRequestHandler<PreferTwoFactorMethodCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly ITwoFactorQueryService _twoFactorQueryService = twoFactorQueryService;
    private readonly ITwoFactorCommandService _twoFactorCommandService = twoFactorCommandService;

    public async Task<Result> Handle(PreferTwoFactorMethodCommand request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var twoFactorMethod = await _twoFactorQueryService.GetByMethodAsync(user.Id, 
            request.PreferredMethod, cancellationToken);

        if (twoFactorMethod is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid 2FA method"
            });
        }

        return await _twoFactorCommandService.SetPreferredMethodAsync(user.Id, twoFactorMethod.Id, cancellationToken);
    }
}

public sealed class PreferTwoMethodCommandValidator : IRequestValidator<PreferTwoFactorMethodCommand>
{
    public async ValueTask<Result> Validate(PreferTwoFactorMethodCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.PreferredMethod == TwoFactorMethod.None)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'preferred_method' is invalid"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}