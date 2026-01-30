using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace eCinema.Server.Hubs;

[Authorize]
public class AuthenticationHub : Hub
{

}