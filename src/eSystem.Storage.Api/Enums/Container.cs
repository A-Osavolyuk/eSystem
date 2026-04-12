using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Storage.Api.Enums;

[JsonConverter(typeof(JsonEnumValueConverter<Container>))]
public enum Container
{
    [EnumValue("avatar")]
    Avatar
}