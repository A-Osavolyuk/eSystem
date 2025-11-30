using System.Text.Json.Serialization;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Core.Common.Responses;

public class UserInfoResponse
{
    [JsonPropertyName(AppClaimTypes.Sub)]
    public required string Subject { get; set; }
    
    [JsonPropertyName(AppClaimTypes.Name)]
    public string? Name { get; set; }
    
    [JsonPropertyName(AppClaimTypes.GivenName)]
    public string? GivenName { get; set; }
    
    [JsonPropertyName(AppClaimTypes.FamilyName)]
    public string? FamilyName { get; set; }
    
    [JsonPropertyName(AppClaimTypes.MiddleName)]
    public string? MiddleName { get; set; }
    
    [JsonPropertyName(AppClaimTypes.Nickname)]
    public string? Nickname { get; set; }
    
    [JsonPropertyName(AppClaimTypes.PreferredUsername)]
    public string? PreferredUsername { get; set; }
    
    [JsonPropertyName(AppClaimTypes.Profile)]
    public string? Profile { get; set; }
    
    [JsonPropertyName(AppClaimTypes.Gender)]
    public string? Gender { get; set; }
    
    [JsonPropertyName(AppClaimTypes.BirthDate)]
    public DateTime? Birthdate { get; set; }
    
    [JsonPropertyName(AppClaimTypes.UpdatedAt)]
    public long? UpdatedAt { get; set; }
    
    [JsonPropertyName(AppClaimTypes.ZoneInfo)]
    public string? Zoneinfo { get; set; }
    
    [JsonPropertyName(AppClaimTypes.Locale)]
    public string? Locale { get; set; }
    
    [JsonPropertyName(AppClaimTypes.Email)]
    public string? Email{ get; set; }
    
    [JsonPropertyName(AppClaimTypes.EmailVerified)]
    public bool? EmailVerified { get; set; }
    
    [JsonPropertyName(AppClaimTypes.PhoneNumber)]
    public string? PhoneNumber { get; set; }
    
    [JsonPropertyName(AppClaimTypes.PhoneNumberVerified)]
    public bool? PhoneNumberVerified { get; set; }
    
    [JsonPropertyName(AppClaimTypes.Address)]
    public string? Address { get; set; }
}