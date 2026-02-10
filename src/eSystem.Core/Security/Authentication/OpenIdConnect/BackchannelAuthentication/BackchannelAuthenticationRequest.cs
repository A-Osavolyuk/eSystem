using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

public sealed class BackchannelAuthenticationRequest
{
    [FromForm(Name = "scope")]
    public required string Scope { get; set; }

    [FromForm(Name = "client_id")]
    public string? ClientId { get; set; }
    
    [FromForm(Name = "client_secret")]
    public string? ClientSecret { get; set; }
    
    [FromForm(Name = "client_assertion")]
    public string? ClientAssertion { get; set; }
    
    [FromForm(Name = "client_assertion_type")]
    public string? ClientAssertionType { get; set; }
    
    [FromForm(Name = "login_hint")]
    public string? LoginHint { get; set; }
    
    [FromForm(Name = "login_token_hint")]
    public string? LoginTokenHint { get; set; }
    
    [FromForm(Name = "id_token_hint")]
    public string? IdTokenHint { get; set; }
    
    [FromForm(Name = "binding_message")]
    public string? BindingMessage { get; set; }
    
    [FromForm(Name = "user_code")]
    public string? UserCode { get; set; }
    
    [FromForm(Name = "requested_expiry")]
    public int? RequestedExpiry { get; set; }
    
    [FromForm(Name = "acr_values")]
    public string? AcrValues { get; set; }
    
    [FromForm(Name = "client_notification_endpoint")]
    public string? ClientNotificationEndpoint { get; set; }
    
}