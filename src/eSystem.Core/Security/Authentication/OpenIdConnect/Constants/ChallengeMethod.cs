using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

public enum ChallengeMethod
{
    [EnumValue("plain")]
    [Obsolete("Use 'S256' instead. 'plain' is not recommended due to security reasons.")]
    Plain,
    
    [EnumValue("S256")]
    S256
}