using eSystem.Core.Security.Identity.Claims;
using Microsoft.AspNetCore.SignalR;

namespace eCinema.Server.Security.Identity;

public class SubjectUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
        => connection.User?.FindFirst(AppClaimTypes.Sub)?.Value;
}