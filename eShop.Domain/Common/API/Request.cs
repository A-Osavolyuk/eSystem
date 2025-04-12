namespace eShop.Domain.Common.API;

public record Request(
    string Url,
    HttpMethods Methods,
    object? Data = null!);