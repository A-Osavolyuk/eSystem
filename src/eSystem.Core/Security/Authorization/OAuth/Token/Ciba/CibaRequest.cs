using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Security.Authorization.OAuth.Token.Ciba;

public sealed class CibaRequest : TokenRequest
{
    [FromForm(Name = "auth_req_id")]
    public required string AuthReqId { get; set; }
}