using eSystem.Core.Enums;
using eSystem.Core.Form;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authorization.OAuth;
using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Server.Security.Authorization.OAuth.Token;

public abstract class TokenRequest : IFormRequest
{
    [FromForm(Name = "grant_type")]
    public required GrantType GrantType { get; set; }
    
    [FromForm(Name = "client_id")]
    public required string ClientId { get; set; }
    
    [FromForm(Name = "client_secret")]
    public string? ClientSecret { get; set; }
    
    [FromForm(Name = "client_assertion")]
    public string? ClientAssertion { get; set; }
    
    [FromForm(Name = "client_assertion_type")]
    public AssertionType? ClientAssertionType { get; set; }

    public virtual Dictionary<string, string> GetForm()
    {
        var form = new Dictionary<string, string>()
        {
            { "grant_type", GrantType.GetString() },
            { "client_id", ClientId }
        };

        if (!string.IsNullOrEmpty(ClientSecret))
            form["client_secret"] = ClientSecret;

        if (!string.IsNullOrEmpty(ClientAssertion))
            form["client_assertion"] = ClientAssertion;

        if (ClientAssertionType.HasValue)
            form["client_assertion_type"] = ClientAssertionType.Value.GetString();
        
        return form;
    }
}