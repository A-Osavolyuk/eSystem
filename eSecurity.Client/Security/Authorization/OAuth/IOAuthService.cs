using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authorization.OAuth;

public interface IOAuthService
{
    public ValueTask<Result> LoadSessionAsync(LoadOAuthSessionRequest request);
}