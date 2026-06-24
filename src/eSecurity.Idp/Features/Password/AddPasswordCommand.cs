using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Authentication.Password;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Server.Exceptions;

namespace eSecurity.Idp.Features.Password;

public sealed class AddPasswordCommand : IRequest<Result>
{
    [JsonPropertyName("password")]
    public string? Password { get; set; }
}

public sealed class AddPasswordCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IPasswordManager passwordManager) : IRequestHandler<AddPasswordCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IPasswordManager _passwordManager = passwordManager;

    public async Task<Result> Handle(AddPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        if (await _passwordManager.HasAsync(user, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidPassword,
                Description = "User already has a password."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ValidationException("Password is required");
        
        return await _passwordManager.AddAsync(user, request.Password, cancellationToken);
    }
}

public sealed class AddPasswordCommandValidator : IRequestValidator<AddPasswordCommand>
{
    public async ValueTask<Result> Validate(AddPasswordCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'password' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}