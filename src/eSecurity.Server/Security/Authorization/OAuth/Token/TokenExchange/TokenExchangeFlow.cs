using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange;

[JsonConverter(typeof(JsonEnumValueConverter<TokenExchangeFlow>))]
public enum TokenExchangeFlow
{
    [EnumValue("delegation")]
    Delegation,
    
    [EnumValue("transformation")]
    Transformation
}