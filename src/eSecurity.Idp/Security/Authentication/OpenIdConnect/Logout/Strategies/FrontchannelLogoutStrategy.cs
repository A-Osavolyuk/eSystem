using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.OpenIdConnect.Client;

namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Logout.Strategies;

public class FrontchannelLogoutStrategy(IClientManager clientManager) : ILogoutStrategy<List<string>>
{
    private readonly IClientManager _clientManager = clientManager;

    public async ValueTask<List<string>> ExecuteAsync(SessionEntity session, CancellationToken cancellationToken)
    {
        var clients = await _clientManager.GetClientsAsync(session, cancellationToken);
        return clients.Where(x => x.AllowFrontChannelLogout)
            .SelectMany(x => x.Uris)
            .Where(x => x.Type == UriType.FrontChannelLogout)
            .Select(x => x.Uri)
            .ToList();
    }
}