namespace eSystem.Core.DTOs;

public class SupplierDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public string? WebsiteUrl { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
}