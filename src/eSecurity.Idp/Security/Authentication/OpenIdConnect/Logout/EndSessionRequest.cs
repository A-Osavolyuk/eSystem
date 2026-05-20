namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Logout;

public sealed class EndSessionRequest
{
    [FromQuery(Name = "id_token_hint")]
    public string? IdTokenHint { get; set; }

    [FromQuery(Name = "post_logout_redirect_uri")]
    public string? PostLogoutRedirectUri { get; set; }

    [FromQuery(Name = "state")]
    public string? State { get; set; }

    [FromQuery(Name = "logout_hint")]
    public string? LogoutHint { get; set; }

    [FromQuery(Name = "client_id")]
    public string? ClientId { get; set; }

    [FromQuery(Name = "ui_locales")]
    public string? UiLocales { get; set; }
}