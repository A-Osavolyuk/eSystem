using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.WebAuthN.Constants;

[JsonConverter(typeof(JsonEnumValueConverter<ClientDataType>))]
public enum ClientDataType
{
    [EnumValue("webauthn.create")]
    Create,
    
    [EnumValue("webauthn.ge")]
    Get
}