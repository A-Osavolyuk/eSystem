namespace eSecurity.Core.Common.Requests;

public sealed class ChangePasskeyNameRequest
{
    public required string Subject { get; set; }
    public required Guid PasskeyId { get; set; }
    public required string DisplayName { get; set; }
}