namespace eShop.Auth.Api.Interfaces;

public interface ILoginSessionManager
{
    public ValueTask<Result> CreateAsync(UserEntity user, HttpContext httpContext,
        LoginStatus status, LoginType type, string? provider = null, CancellationToken cancellationToken = default);
}