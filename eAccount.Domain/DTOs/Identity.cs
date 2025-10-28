namespace eAccount.Domain.DTOs;

public class Identity
{
    public required Dictionary<string, string> Claims { get; set; }
    public required string Scheme { get; set; }
}