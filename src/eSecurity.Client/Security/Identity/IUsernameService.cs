using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Identity;

public interface IUsernameService
{
    public ValueTask<ApiResponse> SetUsernameAsync(SetUsernameRequest request);
    public ValueTask<ApiResponse> ChangeUsernameAsync(ChangeUsernameRequest request);
    public ValueTask<ApiResponse> CheckUsernameAsync(CheckUsernameRequest request);
}