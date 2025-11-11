using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Services.Interfaces;

public interface ILinkedAccountService
{
    public ValueTask<HttpResponse> DisconnectAsync(DisconnectLinkedAccountRequest request);
}