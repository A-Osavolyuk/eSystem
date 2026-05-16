using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Server.Security.Authorization.OAuth.Token.AuthorizationCode;

public sealed class AuthorizationCodeRequest : TokenRequest
{
    [FromForm(Name = "code")]
    public string? Code { get; init; }
    
    [FromForm(Name = "code_verifier")]
    public string? CodeVerifier { get; init; }
    
    [FromForm(Name = "redirect_uri")]
    public string? RedirectUri { get; init; }

    public override Dictionary<string, string> GetForm()
    {
        var form = base.GetForm();
        if (!string.IsNullOrEmpty(Code))
            form["code"] = Code;

        if (!string.IsNullOrEmpty(CodeVerifier))
            form["code_verifier"] = CodeVerifier;

        if (!string.IsNullOrEmpty(RedirectUri))
            form["redirect_uri"] = RedirectUri;

        return form;
    }
}