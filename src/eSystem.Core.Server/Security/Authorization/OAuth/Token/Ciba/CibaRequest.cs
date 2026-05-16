using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Server.Security.Authorization.OAuth.Token.Ciba;

public sealed class CibaRequest : TokenRequest
{
    [FromForm(Name = "auth_req_id")]
    public required string AuthReqId { get; init; }

    public override Dictionary<string, string> GetForm()
    {
        var form = base.GetForm();
        if (!string.IsNullOrEmpty(AuthReqId))
            form["auth_req_id"] = AuthReqId;
        
        return form;
    }
}