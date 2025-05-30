using eShop.Domain.Abstraction.Messaging;

namespace eShop.EmailSender.Api.Mapping;

public static class Mapper
{
    public static MessageOptions Map(EmailMessage email)
    {
        return new MessageOptions()
        {
            Subject = email.Credentials.Subject,
            To = email.Credentials.To,
            UserName = email.Credentials.UserName
        };
    }
}