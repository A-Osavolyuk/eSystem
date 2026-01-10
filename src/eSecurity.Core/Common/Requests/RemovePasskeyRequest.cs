namespace eSecurity.Core.Common.Requests;

public class RemovePasskeyRequest
{
    public Guid UserId { get; set; }
    public Guid PasskeyId { get; set; }
}