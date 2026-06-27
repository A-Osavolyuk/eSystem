using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.WebAuthN.Constants;

[JsonConverter(typeof(CredentialTransport))]
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