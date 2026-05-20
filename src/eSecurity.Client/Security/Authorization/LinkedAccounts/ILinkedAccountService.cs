using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;

namespace eSecurity.Client.Security.Authorization.LinkedAccounts;

public interface ILinkedAccountService
{
    public ValueTask<ApiResponse> DisconnectAsync(DisconnectLinkedAccountRequest request);
}