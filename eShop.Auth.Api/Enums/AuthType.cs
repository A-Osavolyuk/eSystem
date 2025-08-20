using System.Runtime.Serialization;

namespace eShop.Auth.Api.Enums;

public enum AuthType
{
    [EnumMember(Value = "webauthn.create")]
    Create,
    
    [EnumMember(Value = "webauthn.get")]
    Get
}