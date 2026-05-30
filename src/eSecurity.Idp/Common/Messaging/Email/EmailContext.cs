namespace eSecurity.Idp.Common.Messaging.Email;

public abstract class EmailContext
{
    public required string To { get; set; }
    public required string Subject { get; set; }
}