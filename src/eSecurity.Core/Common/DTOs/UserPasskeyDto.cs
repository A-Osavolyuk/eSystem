using System.Globalization;

namespace eSecurity.Core.Common.DTOs;

public class UserPasskeyDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("current_key")]
    public bool CurrentKey { get; set; }
    
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;
    
    [JsonPropertyName("last_seen_at")]
    public DateTimeOffset? LastSeenAt { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }
    
    public string GetInfo()
    {
        var formattedDate = CreatedAt?.ToString("MMM d, yyyy", CultureInfo.InvariantCulture)!;
        var addedText = $"Added on {formattedDate}";

        if (!LastSeenAt.HasValue) return addedText;
        
        var difference = DateTimeOffset.UtcNow - LastSeenAt.Value;
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