using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Authentication.Password;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Password;

public record AddPasswordCommand : IRequest<Result>
{
    [JsonPropertyName("password")]
    public required string Password { get; set; }
}

public class AddPasswordCommandHandler(
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
        
        return await _passwordManager.AddAsync(user, request.Password, cancellationToken);
    }
}