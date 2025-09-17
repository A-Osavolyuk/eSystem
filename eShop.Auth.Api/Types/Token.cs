namespace eShop.Auth.Api.Types;

public class Token
{
    public required Guid Id { get; set; }
    public required string Value { get; set; }
    public required DateTime ExpireDate { get; set; }
}