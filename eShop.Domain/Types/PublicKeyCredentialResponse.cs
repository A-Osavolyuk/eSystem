namespace eShop.Domain.Types;

public class PublicKeyCredentialResponse
{
    public required string Id { get; set; }
    public required string RawId { get; set; }
    public required string Type { get; set; }
    public required CredentialResponse Response { get; set; }
}