namespace eShop.Domain.Types;

public class CredentialResponse
{
    public required string AttestationObject { get; set; }
    public required string ClientDataJson { get; set; }
}