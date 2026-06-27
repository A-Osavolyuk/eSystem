using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Authentication.Session;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Users;

public sealed class SetUsernameCommand : IRequest<Result>
{
    [JsonPropertyName("session_id")]
    public Guid SessionId { get; set; }
    
    [JsonPropertyName("username")]
    public string? Username { get; set; }
}

public sealed class SetUsernameCommandHandler(
    ISessionQueryService sessionQueryService,
    IUserQueryService userQueryService,
    IUserCommandService userCommandService) : IRequestHandler<SetUsernameCommand, Result>
{
    private readonly ISessionQueryService _sessionQueryService = sessionQueryService;
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IUserCommandService _userCommandService = userCommandService;

    public async Task<Result> Handle(SetUsernameCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionQueryService.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Session not found"
            });
        }
        
        var user = await _userQueryService.GetByIdAsync(session.UserId, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "User not found"
            });
        }

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

        return await _userCommandService.ChangeUsernameAsync(user.Id, request.Username, cancellationToken);
    }
}

public sealed class SetUsernameCommandValidator : IRequestValidator<SetUsernameCommand>
{
    public async ValueTask<Result> Validate(SetUsernameCommand request, CancellationToken cancellationToken)
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