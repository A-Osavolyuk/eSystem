namespace eSecurity.Core.Common.Requests;

public sealed class GenerateCreationOptionsRequest
{
    public required string Subject { get; set; }
    public required string DisplayName { get; set; }
}