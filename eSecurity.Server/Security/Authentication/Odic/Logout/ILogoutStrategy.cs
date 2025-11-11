namespace eSecurity.Server.Security.Authentication.Odic.Logout;

public interface ILogoutStrategy
{
    public ValueTask<Result> ExecuteAsync(LogoutPayload payload, CancellationToken cancellationToken = default);
}

public abstract class LogoutPayload() {}