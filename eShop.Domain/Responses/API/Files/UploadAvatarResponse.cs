using eShop.Domain.Abstraction.Responses;

namespace eShop.Domain.Responses.API.Files;

public class UploadAvatarResponse : ResponseBase
{
    public string Uri { get; set; } = string.Empty;
}