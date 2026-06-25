using System.Text.Json.Serialization;
using eSecurity.Core.Responses;
using eSecurity.Idp.Security.Authentication.Lockout;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Server.Exceptions;

namespace eSecurity.Idp.Features.Account;

public sealed class CheckAccountCommand : IRequest<Result>
{
    [JsonPropertyName("login")]
    public string? Login { get; set; }
}

public sealed class CheckAccountCommandHandler(
    IUserQueryService userQueryService,
    ILockoutManager lockoutManager) : IRequestHandler<CheckAccountCommand, Result>
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly ILockoutManager _lockoutManager = lockoutManager;

    public async Task<Result> Handle(CheckAccountCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Login))
            throw new ValidationException("UserCode is required");
        
        var user = await _userQueryService.GetByLoginAsync(request.Login, cancellationToken);
        if (user is null)
        {
            var result = new CheckAccountResponse { Exists = false };
            return Results.Success(SuccessCodes.Ok, result);
        }

        var lockoutState = await _lockoutManager.GetAsync(user, cancellationToken);
        if (lockoutState is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.InvalidLockoutState,
                Description = "Invalid state"
            });
        }

        var response = new CheckAccountResponse
        {
            Exists = true
        };

        return Results.Success(SuccessCodes.Ok, response);
    }
}

public sealed class CheckAccountCommandValidator : IRequestValidator<CheckAccountCommand>
{
    public async ValueTask<Result> Validate(CheckAccountCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Login))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'login' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}