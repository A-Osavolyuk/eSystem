using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authorization.LinkedAccounts;

public interface ILinkedAccountService
{
    public ValueTask<HttpResponse> DisconnectAsync(DisconnectLinkedAccountRequest request);
}