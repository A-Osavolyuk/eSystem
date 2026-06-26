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
    IPasswordCommandService passwordCommandService) : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IPasswordCommandService _passwordCommandService = passwordCommandService;

    async Task<Result> IRequestHandler<ChangePasswordCommand, Result>.Handle(ChangePasswordCommand request,
            CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        
        if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            throw new ValidationException("CurrentPassword is required");
        
        if (string.IsNullOrWhiteSpace(request.NewPassword))
            throw new ValidationException("NewPassword is required");
        
        return await _passwordCommandService.ChangeAsync(user.Id, request.CurrentPassword, 
            request.NewPassword, cancellationToken);
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