using eSystem.EmailSender.Api.Options;

namespace eSystem.EmailSender.Api.Interfaces;

public interface IEmailService
{
    public ValueTask SendMessageAsync(string htmlBody, MessageOptions messageOptions);
}