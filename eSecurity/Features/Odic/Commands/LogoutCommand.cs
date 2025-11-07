using eSecurity.Security.Authentication.Odic.Logout;
using eSecurity.Security.Authentication.Odic.Logout.Strategies;

namespace eSecurity.Features.Odic.Commands;

public class LogoutCommand() : IRequest<Result>
{
    public required LogoutType Type { get; set; }
    public string? IdTokenHint { get; set; }
    public string? PostLogoutRedirectUri { get; set; }
    public string? State { get; set; }
    public string? ClientId { get; set; }
    public string? LogoutHint { get; set; }
    public string? UiLocales { get; set; }
}

public class LogoutCommandHandler(ILogoutStrategyResolver resolver) : IRequestHandler<LogoutCommand, Result>
{
    private readonly ILogoutStrategyResolver resolver = resolver;

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Type switch
        {
            LogoutType.Manual => new ManualLogoutPayload(),
            _ => throw new NotSupportedException("Unsupported logout type")
        };

        var strategy = resolver.Resolve(request.Type);
        return await strategy.ExecuteAsync(payload, cancellationToken);
    }
}