using eSystem.Core.Security.Identity.Claims;
using Microsoft.AspNetCore.SignalR;

namespace eCinema.Server.Hubs;

public class SubUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
        => connection.User?.FindFirst(AppClaimTypes.Sub)?.Value;
}