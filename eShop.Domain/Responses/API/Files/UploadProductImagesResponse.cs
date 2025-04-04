using eShop.Domain.Abstraction.Responses;

namespace eShop.Domain.Responses.Api.Files;

public class UploadProductImagesResponse : ResponseBase
{
    public List<string> Images { get; set; } = new List<string>();
}