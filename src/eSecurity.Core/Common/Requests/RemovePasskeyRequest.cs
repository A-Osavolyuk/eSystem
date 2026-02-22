namespace eSecurity.Core.Common.Requests;

public sealed class RemovePasskeyRequest
{
    public required string Subject { get; set; }
    public Guid PasskeyId { get; set; }
}