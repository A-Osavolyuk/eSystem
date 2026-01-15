using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Security.Authentication.Oidc.User;

public class UserInfoRequest
{
    [FromForm(Name = "access_token")]
    public string? AccessToken { get; set; }
}