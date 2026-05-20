using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;

namespace eSecurity.Client.Security.Identity;

public interface IUsernameService
{
    public ValueTask<ApiResponse> SetUsernameAsync(SetUsernameRequest request);
}