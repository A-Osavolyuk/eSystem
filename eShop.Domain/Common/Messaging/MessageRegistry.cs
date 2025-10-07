using eShop.Domain.Abstraction.Messaging;

namespace eShop.Domain.Common.Messaging;

public class MessageRegistry
{
    private Dictionary<(SenderType, PurposeType, ActionType), Type> configurations = [];

    public void Add<TMessageType>(SenderType sender, PurposeType purpose, ActionType action)
        where TMessageType : Message
    {
        if (configurations.ContainsKey((sender, purpose, action)))
            throw new Exception("Message with same key is already registered");

        configurations[(sender, purpose, action)] = typeof(TMessageType);
    }

    public Message? Create(MessageMetadata metadata, Dictionary<string, string> payload)
    {
        if (!configurations.TryGetValue((metadata.Sender, metadata.Purpose, metadata.Action), out var messageType))
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
    public required ActionType Action { get; set; }
    public required PurposeType Purpose { get; set; }
}