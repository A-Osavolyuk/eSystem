using eSystem.Core.Security.Identity.Claims;
using Microsoft.AspNetCore.SignalR;

namespace eSecurity.Client.BFF.Security.Identity;

public sealed class SubjectUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
        => connection.User?.FindFirst(AppClaimTypes.Sub)?.Value;
}