using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.WebAuthN.Constants;

[JsonConverter(typeof(JsonEnumValueConverter<CredentialTransport>))]
public enum CredentialTransport
{
    [EnumValue("internal")]
    Internal,
    
    [EnumValue("usb")]
    Usb,
    
    [EnumValue("nfc")]
    Nfc,
    
    [EnumValue("ble")]
    Bluetooth
}