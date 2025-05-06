namespace eShop.Domain.Types;

public record User(
    string Email,
    string UserName,
    Guid Id);
