namespace eShop.Domain.Types;

public class PublicKeyCredentialCreationOptionsResponse
{
    public required string Id { get; set; }
    public required string RawId { get; set; }
    public required string Type { get; set; }
    public required CredentialResponse Response { get; set; }
}