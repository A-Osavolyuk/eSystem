using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Authentication.Password;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Password;

public sealed record ChangePasswordCommand : IRequest<Result>
{
    [JsonPropertyName("current_password")]
    public required string CurrentPassword { get; set; }
    
    [JsonPropertyName("new_password")]
    public required string NewPassword { get; set; }
}

public sealed class ChangePasswordCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IPasswordManager passwordManager) : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IPasswordManager _passwordManager = passwordManager;

    async Task<Result> IRequestHandler<ChangePasswordCommand, Result>.Handle(ChangePasswordCommand request,
            CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        if (!await _passwordManager.HasAsync(user, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidPassword,
                Description = "User does not have a password."
            });
        }

        if (!await _passwordManager.CheckAsync(user, request.CurrentPassword, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidPassword,
                Description = "Invalid password."
            });
        }
        
        var result = await _passwordManager.ChangeAsync(user, request.NewPassword, cancellationToken);

        return result;
    }
}