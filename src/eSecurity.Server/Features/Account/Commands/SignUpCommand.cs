using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Identity.SignUp;
using eSecurity.Server.Security.Identity.SignUp.Strategies;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Features.Account.Commands;

public sealed record SignUpCommand(SignUpRequest Request) : IRequest<Result>;

public sealed class SignUpCommandHandler(ISignUpResolver resolver) : IRequestHandler<SignUpCommand, Result>
{
    private readonly ISignUpResolver _resolver = resolver;

    public async Task<Result> Handle(SignUpCommand request,
        CancellationToken cancellationToken)
    {
        var strategy = _resolver.Resolve(SignUpType.Manual);

        var payload = new ManualSignUpPayload
        {
            Username = request.Request.Username,
            Email = request.Request.Email,
            Password = request.Request.Password
        };
        
        return await strategy.ExecuteAsync(payload, cancellationToken);
    }
}