using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authorization.LinkedAccounts;

public interface ILinkedAccountService
{
    public ValueTask<Result> DisconnectAsync(DisconnectLinkedAccountRequest request);
}