using eSystem.Core.Primitives;

namespace eSystem.SmsSender.Api.Interfaces;

public interface ISmsService
{
    public Task<Result> SendMessageAsync(string phoneNumber, string message);
}