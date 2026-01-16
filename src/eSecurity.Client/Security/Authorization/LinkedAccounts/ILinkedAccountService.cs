using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authorization.LinkedAccounts;

public interface ILinkedAccountService
{
    public ValueTask<ApiResponse> DisconnectAsync(DisconnectLinkedAccountRequest request);
}