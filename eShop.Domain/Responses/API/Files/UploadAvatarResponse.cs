namespace eShop.Domain.Responses.Api.Files;

public class UploadAvatarResponse : ResponseBase
{
    public string Uri { get; set; } = string.Empty;
}