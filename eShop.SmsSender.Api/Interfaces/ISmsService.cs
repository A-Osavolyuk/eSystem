using eShop.Domain.Common.API;

namespace eShop.SmsSender.Api.Interfaces;

public interface ISmsService
{
    public Task<Result> SendMessageAsync(string phoneNumber, string message);
}