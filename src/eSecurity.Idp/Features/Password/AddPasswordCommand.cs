using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Authentication.Password;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Password;

public sealed class AddPasswordCommand : IRequest<Result>
{
    [JsonPropertyName("password")]
    public string? Password { get; set; }
}

public sealed class AddPasswordCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IPasswordCommandService passwordCommandService) : IRequestHandler<AddPasswordCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IPasswordCommandService _passwordCommandService = passwordCommandService;

    public async Task<Result> Handle(AddPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ValidationException("Password is required");
        
        return await _passwordCommandService.AddAsync(user.Id, request.Password, cancellationToken);
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