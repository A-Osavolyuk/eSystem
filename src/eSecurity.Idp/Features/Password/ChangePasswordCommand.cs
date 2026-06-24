using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Authentication.Password;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Server.Exceptions;

namespace eSecurity.Idp.Features.Password;

public sealed class ChangePasswordCommand : IRequest<Result>
{
    [JsonPropertyName("current_password")]
    public string? CurrentPassword { get; set; }
    
    [JsonPropertyName("new_password")]
    public string? NewPassword { get; set; }
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

        if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            throw new ValidationException("CurrentPassword is required");
        
        if (!await _passwordManager.CheckAsync(user, request.CurrentPassword, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidPassword,
                Description = "Invalid password."
            });
        }
        
        if (string.IsNullOrWhiteSpace(request.NewPassword))
            throw new ValidationException("NewPassword is required");
        
        return await _passwordManager.ChangeAsync(user, request.NewPassword, cancellationToken);
    }
}

public sealed class ChangePasswordCommandValidator : IRequestValidator<ChangePasswordCommand>
{
    public async ValueTask<Result> Validate(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.CurrentPassword))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'current_password' is required"
            });
        }
        
        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'new_password' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}