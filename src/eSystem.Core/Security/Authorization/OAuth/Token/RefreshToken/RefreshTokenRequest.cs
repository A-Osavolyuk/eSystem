using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Security.Authorization.OAuth.Token.RefreshToken;

public sealed class RefreshTokenRequest : TokenRequest
{
    [FromForm(Name = "refresh_token")]
    public string? RefreshToken { get; set; }
}