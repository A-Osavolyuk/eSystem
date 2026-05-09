using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Security.Authorization.OAuth.Token.RefreshToken;

public sealed class RefreshTokenRequest : TokenRequest
{
    [FromForm(Name = "refresh_token")]
    public string? RefreshToken { get; init; }

    public override Dictionary<string, string> GetForm()
    {
        var form = base.GetForm();
        if (!string.IsNullOrEmpty(RefreshToken))
            form["refresh_token"] = RefreshToken;

        return form;
    }
}