using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Security.Authorization.OAuth.Token.ClientCredentials;

public sealed class ClientCredentialsRequest : TokenRequest
{
    [FromForm(Name = "scope")]
    public string? Scope { get; set; }
}