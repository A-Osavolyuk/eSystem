namespace eShop.Auth.Api.Messages;

public class MessageRegistry
{
    private Dictionary<(SenderType, CodeResource, CodeType), Type> configurations = [];

    public void Add<TMessageType>(SenderType sender, CodeResource resource, CodeType type)
    {
        if (configurations.ContainsKey((sender, resource, type))) 
            throw new Exception("Message with same key is already registered");
        
        configurations[(sender, resource, type)] = typeof(TMessageType);
    }

    public Message? Create(MessageMetadata metadata, Dictionary<string, string> payload)
    {
        if (!configurations.TryGetValue((metadata.Sender, metadata.Resource, metadata.Type), out var messageType))
            return null;

        var message = (Message?)Activator.CreateInstance(messageType);
        if (message == null) return null;

        message?.Initialize(payload);
        return message;
    }
}

public class MessageMetadata
{
    public required SenderType Sender { get; set; }
    public required CodeType Type { get; set; }
    public required CodeResource Resource { get; set; }
}