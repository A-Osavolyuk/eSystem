using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Primitives.Enums;

[JsonConverter(typeof(JsonEnumValueConverter<RedirectionCode>))]
public enum RedirectionCode
{
    [EnumValue("multiple_choices")]
    MultipleChoices = 300,
    
    [EnumValue("moved_permanently")]
    MovedPermanently = 301,
    
    [EnumValue("found", true)]
    [EnumValue("moved_temporarily")]
    Found = 302,
    
    [EnumValue("see_other")]
    SeeOther = 303,
    
    [EnumValue("not_modified")]
    NotModified = 304,
    
    [EnumValue("use_proxy")]
    UseProxy = 305,
    
    [EnumValue("unused")]
    Unused = 306,
    
    [EnumValue("temporary_redirect")]
    TemporaryRedirect = 307,
    
    [EnumValue("permanent_redirect")]
    PermanentRedirect = 308,
}