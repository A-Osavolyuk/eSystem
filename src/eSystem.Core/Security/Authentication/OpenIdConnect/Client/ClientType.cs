using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.Client;

[JsonConverter(typeof(JsonEnumValueConverter<ClientType>))]
public enum ClientType
{
    [EnumValue("confidential")]
    Confidential,
    
    [EnumValue("public")]
    Public
}