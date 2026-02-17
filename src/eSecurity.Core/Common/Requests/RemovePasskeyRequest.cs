namespace eSecurity.Core.Common.Requests;

public class RemovePasskeyRequest
{
    public required string Subject { get; set; }
    public Guid PasskeyId { get; set; }
}