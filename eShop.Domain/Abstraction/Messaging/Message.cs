namespace eShop.Domain.Abstraction.Messaging;

public abstract class Message<TCredentials> where TCredentials : MessageCredentials
{
    public required TCredentials Credentials { get; set; }
}