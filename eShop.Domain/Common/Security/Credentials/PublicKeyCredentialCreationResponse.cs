namespace eShop.Domain.Common.Security.Credentials;

public class PublicKeyCredentialCreationResponse
{
    public required string Id { get; set; }
    public required string RawId { get; set; }
    public required string Type { get; set; }
    public required CredentialResponse Response { get; set; }
}

public class CredentialResponse
{
    public required string AttestationObject { get; set; }
    public required string ClientDataJson { get; set; }
}