using eSecurity.Security.Identity.SignUp;
using eSecurity.Security.Identity.SignUp.Strategies;

namespace eSecurity.Features.Security.Commands;

public sealed record SignUpCommand() : IRequest<Result>
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class SignUpCommandHandler(ISignUpResolver resolver) : IRequestHandler<SignUpCommand, Result>
{
    private readonly ISignUpResolver resolver = resolver;

    public async Task<Result> Handle(SignUpCommand request,
        CancellationToken cancellationToken)
    {
        var strategy = resolver.Resolve(SignUpType.Manual);

        var payload = new ManualSignUpPayload()
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password
        };
        
        return await strategy.ExecuteAsync(payload, cancellationToken);
    }
}