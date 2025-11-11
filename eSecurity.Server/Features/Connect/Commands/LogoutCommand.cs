using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authentication.Odic.Logout;
using eSecurity.Server.Security.Authentication.Odic.Logout;
using eSecurity.Server.Security.Authentication.Odic.Logout.Strategies;

namespace eSecurity.Server.Features.Connect.Commands;

public record LogoutCommand(LogoutRequest Request) : IRequest<Result>;

public class LogoutCommandHandler(ILogoutStrategyResolver resolver) : IRequestHandler<LogoutCommand, Result>
{
    private readonly ILogoutStrategyResolver resolver = resolver;

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Request.Type switch
        {
            LogoutType.Manual => new ManualLogoutPayload(),
            _ => throw new NotSupportedException("Unsupported logout type")
        };

        var strategy = resolver.Resolve(request.Request.Type);
        return await strategy.ExecuteAsync(payload, cancellationToken);
    }
}