using eSecurity.Security.Authentication.Odic.Session;

namespace eSecurity.Features.Odic.Commands;

public class LogoutCommand() : IRequest<Result>
{
    public string? IdTokenHint { get; set; }
    public string? PostLogoutRedirectUri { get; set; }
    public string? State { get; set; }
    public string? ClientId { get; set; }
    public string? LogoutHint { get; set; }
    public string? UiLocales { get; set; }
}

public class LogoutCommandHandler(ISessionManager sessionManager) : IRequestHandler<LogoutCommand, Result>
{
    private readonly ISessionManager sessionManager = sessionManager;

    public Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Result());
    }
}