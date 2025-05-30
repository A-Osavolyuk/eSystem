namespace eShop.Domain.Abstraction.Messaging;

public abstract class MessageBase<TCredentials> where TCredentials : MessageCredentials
{
    public required TCredentials Credentials { get; set; }
}