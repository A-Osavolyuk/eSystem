using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Http;

[JsonConverter(typeof(JsonEnumValueConverter<HttpMethods>))]
public enum HttpMethods
{
    [EnumValue("GET")]
    Get,
    
    [EnumValue("POST")]
    Post,
    
    [EnumValue("DELETE")]
    Delete,
    
    [EnumValue("PUT")]
    Put,
    
    [EnumValue("PATCH")]
    Patch,
    
    [EnumValue("HEAD")]
    Head,
    
    [EnumValue("CONNECT")]
    Connect,
    
    [EnumValue("OPTIONS")]
    Options,
    
    [EnumValue("TRACE")]
    Trace
}