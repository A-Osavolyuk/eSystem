using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Identity.SignUp;
using eSecurity.Idp.Security.Identity.SignUp.Strategies;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Features.Account;

public sealed record SignUpCommand : IRequest<Result>
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("username")]
    public required string Username { get; set; }
    
    [JsonPropertyName("password")]
    public required string Password { get; set; }
}

public sealed class SignUpCommandHandler(ISignUpResolver resolver) : IRequestHandler<SignUpCommand, Result>
{
    private readonly ISignUpResolver _resolver = resolver;

    public async Task<Result> Handle(SignUpCommand request,
        CancellationToken cancellationToken)
    {
        var strategy = _resolver.Resolve(SignUpType.Manual);

        var payload = new ManualSignUpPayload
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password
        };
        
        return await strategy.ExecuteAsync(payload, cancellationToken);
    }
}