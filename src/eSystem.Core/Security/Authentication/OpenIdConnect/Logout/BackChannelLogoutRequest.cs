using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.Logout;

public class BackChannelLogoutRequest
{
    [FromForm(Name = "logout_token")]
    public required string LogoutToken { get; set; }
}