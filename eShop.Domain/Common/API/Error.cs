namespace eShop.Domain.Common.Api;

public class Error
{
    public ErrorCode Code { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? Details { get; init; }
}