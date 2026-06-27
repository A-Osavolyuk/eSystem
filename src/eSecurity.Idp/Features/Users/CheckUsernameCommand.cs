using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Users;

public sealed record CheckUsernameCommand : IRequest<Result>
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }
}

public sealed class CheckUsernameCommandHandler(
    IUserQueryService userQueryService) : IRequestHandler<CheckUsernameCommand, Result>
{
    private readonly IUserQueryService _userQueryService = userQueryService;

    public async Task<Result> Handle(CheckUsernameCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            throw new ValidationException("Username is required");
        
        if (await _userQueryService.ExistsAsync(request.Username, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.UsernameTaken,
                Description = "The username is already taken."
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}

public sealed class CheckUsernameCommandValidator : IRequestValidator<CheckUsernameCommand>
{
    public async ValueTask<Result> Validate(CheckUsernameCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'username' is required"
            });
        }

        return Results.Success(SuccessCodes.Ok);
    }
}