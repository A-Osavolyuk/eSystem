using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Idp.Security.Authorization.OAuth.Token.TokenExchange;

[JsonConverter(typeof(JsonEnumValueConverter<TokenExchangeFlow>))]
public enum TokenExchangeFlow
{
    [EnumValue("delegation")]
    Delegation,
    
    [EnumValue("transformation")]
    Transformation
}