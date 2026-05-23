namespace eSecurity.Idp.Security.Authentication.EndSession;

public sealed class ConfirmEndSessionRequest
{
    [FromQuery(Name = "end_session_request_id")]
    public Guid RequestId { get; set; }

    [FromQuery(Name = "state")]
    public string? State { get; set; }
}