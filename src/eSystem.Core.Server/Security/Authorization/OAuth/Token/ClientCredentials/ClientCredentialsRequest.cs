using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Server.Security.Authorization.OAuth.Token.ClientCredentials;

public sealed class ClientCredentialsRequest : TokenRequest
{
    [FromForm(Name = "scope")]
    public string? Scope { get; init; }

    public override Dictionary<string, string> GetForm()
    {
        var form = base.GetForm();
        if (!string.IsNullOrEmpty(Scope))
            form["scope"] = Scope;

        return form;
    }
}