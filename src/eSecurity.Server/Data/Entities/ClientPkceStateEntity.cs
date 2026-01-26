namespace eSecurity.Server.Data.Entities;

public class ClientPkceStateEntity
{
    public Guid ClientId { get; set; }
    public Guid SessionId { get; set; }

    public DateTimeOffset VerificationDate { get; set; }

    public ClientEntity Client { get; set; } = null!;
    public SessionEntity Session { get; set; } = null!;
}