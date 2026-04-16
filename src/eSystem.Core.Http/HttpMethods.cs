using System.Text.Json.Serialization;
using eSystem.Core.Data.Conversion;
using eSystem.Core.Enums;

namespace eSystem.Core.Http;

[JsonConverter(typeof(EnumValueConverter<HttpMethods>))]
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