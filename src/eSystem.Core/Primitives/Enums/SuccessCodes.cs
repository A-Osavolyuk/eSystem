using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Primitives.Enums;

[JsonConverter(typeof(JsonEnumValueConverter<SuccessCodes>))]
public enum SuccessCodes
{
    [EnumValue("ok")]
    Ok = 200,
    
    [EnumValue("created")]
    Created = 201,
    
    [EnumValue("accepted")]
    Accepted = 202,
    
    [EnumValue("non_authoritative_information")]
    NonAuthoritativeInformation = 203,
    
    [EnumValue("no_content")]
    NoContent = 204,
    
    [EnumValue("reset_content")]
    ResetContent = 205,
    
    [EnumValue("partial_content")]
    PartialContent = 206,
    
    [EnumValue("multi_status")]
    MultiStatus = 207,
    
    [EnumValue("already_reported")]
    AlreadyReported = 208,
    
    [EnumValue("im_used")]
    ImUsed = 226
}