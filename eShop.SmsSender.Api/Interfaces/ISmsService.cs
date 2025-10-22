using eShop.Domain.Common.Results;

namespace eShop.SmsSender.Api.Interfaces;

public interface ISmsService
{
    public Task<Result> SendMessageAsync(string phoneNumber, string message);
}