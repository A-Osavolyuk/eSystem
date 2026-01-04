using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Identity;

public interface IUsernameService
{
    public ValueTask<HttpResponse> SetUsernameAsync(SetUsernameRequest request);
    public ValueTask<HttpResponse> ChangeUsernameAsync(ChangeUsernameRequest request);
}