using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSystem.Storage.Api.Enums;

[JsonConverter(typeof(JsonEnumValueConverter<Container>))]
public enum Container
{
    [EnumValue("avatar")]
    Avatar
}