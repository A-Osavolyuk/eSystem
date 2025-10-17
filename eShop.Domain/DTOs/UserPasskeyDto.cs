using System.Globalization;

namespace eShop.Domain.DTOs;

public class UserPasskeyDto
{
    public Guid Id { get; set; }
    public bool CurrentKey { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public DateTimeOffset? LastSeenDate { get; set; }
    public DateTimeOffset? CreateDate { get; set; }
    
    public string GetInfo()
    {
        var formattedDate = CreateDate?.ToString("MMM d, yyyy", CultureInfo.InvariantCulture)!;
        var addedText = $"Added on {formattedDate}";

        if (!LastSeenDate.HasValue) return addedText;
        
        var difference = DateTimeOffset.UtcNow - LastSeenDate.Value;
        var totalHours = difference.TotalHours;
        var lastSeenText = totalHours switch
        {
            < 1 => "Last used less than an hour ago",
            1 or < 2 => "Last used about an hour ago",
            _ => $"Last used about {Math.Truncate(totalHours)} hours ago"
        };

        return $"{addedText} | {lastSeenText}";
    }
}