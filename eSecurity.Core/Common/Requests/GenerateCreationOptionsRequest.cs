namespace eSecurity.Core.Common.Requests;

public class GenerateCreationOptionsRequest
{
    public required Guid UserId { get; set; }
    public required string DisplayName { get; set; }
}