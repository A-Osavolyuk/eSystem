using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.User;

public class UserInfoRequest
{
    [FromForm(Name = "access_token")]
    public string? AccessToken { get; set; }
}