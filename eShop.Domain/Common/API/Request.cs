namespace eShop.Domain.Common.API;

public record Request(
    string Url,
    HttpMethod Method,
    object? Data = null!);