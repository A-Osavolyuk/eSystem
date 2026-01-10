using Microsoft.AspNetCore.Mvc;

namespace eSecurity.Core.Common.Requests;

public class UserInfoRequest
{
    [FromForm(Name = "access_token")]
    public string? AccessToken { get; set; }
}