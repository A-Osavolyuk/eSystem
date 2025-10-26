namespace eSystem.Core.DTOs;

public class TypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public CategoryDto Category { get; set; } = new();
}