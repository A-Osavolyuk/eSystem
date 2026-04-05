using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Client;

[JsonConverter(typeof(JsonEnumValueStringConverter<UriType>))]
public enum UriType
{
    [EnumValue("redirect")]
    Redirect,
    
    [EnumValue("post_logout_redirect")]
    PostLogoutRedirect,
    
    [EnumValue("frontchannel_logout")]
    FrontChannelLogout,
    
    [EnumValue("backchannel_logout")]
    BackChannelLogout,
    
    [EnumValue("notification_endpoint")]
    NotificationEndpoint,
    
    [EnumValue("logo_uri")]
    LogoUri,
    
    [EnumValue("client_uri")]
    ClientUri
}