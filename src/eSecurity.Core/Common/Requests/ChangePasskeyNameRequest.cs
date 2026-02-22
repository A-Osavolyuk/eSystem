namespace eSecurity.Core.Common.Requests;

public sealed class ChangePasskeyNameRequest
{
    public required Guid PasskeyId { get; set; }
    public required string DisplayName { get; set; }
}