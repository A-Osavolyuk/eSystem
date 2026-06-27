using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Identity.SignUp;
using eSecurity.Idp.Security.Identity.SignUp.Strategies;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Account;

public sealed record SignUpCommand : IRequest<Result>
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    
    [JsonPropertyName("password")]
    public string? Password { get; set; }
}

public sealed class SignUpCommandHandler(ISignUpStrategyResolver strategyResolver) : IRequestHandler<SignUpCommand, Result>
{
    private readonly ISignUpStrategyResolver _strategyResolver = strategyResolver;

    public async Task<Result> Handle(SignUpCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email is required");
        
        if (string.IsNullOrWhiteSpace(request.Username))
            throw new ValidationException("Username is required");
        
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ValidationException("Password is required");
        
        var payload = new ManualSignUpPayload
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password
        };
        
        var strategy = _strategyResolver.Resolve(payload);
        return await strategy.ExecuteAsync(payload, cancellationToken);
    }
}

public sealed class SignUpCommandValidator : IRequestValidator<SignUpCommand>
{
    public async ValueTask<Result> Validate(SignUpCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'email' is required"
            });
        }
        
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'password' is required"
            });
        }
        
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