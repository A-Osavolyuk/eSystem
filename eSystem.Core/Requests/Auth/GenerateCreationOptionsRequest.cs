namespace eSystem.Core.Requests.Auth;

public class GenerateCreationOptionsRequest
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}