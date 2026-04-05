using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange;

[JsonConverter(typeof(JsonEnumValueStringConverter<TokenExchangeFlow>))]
public enum TokenExchangeFlow
{
    [EnumValue("delegation")]
    Delegation,
    
    [EnumValue("transformation")]
    Transformation
}