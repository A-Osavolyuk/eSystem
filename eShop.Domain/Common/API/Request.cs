namespace eShop.Domain.Common.Api;

public record Request(
    string Url,
    HttpMethods Methods,
    object? Data = null!);