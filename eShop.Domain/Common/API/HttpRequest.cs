namespace eShop.Domain.Common.API;

public record HttpRequest(
    string Url,
    HttpMethod Method,
    object? Data = null!);