namespace eShop.Domain.Requests.API.Storage;

public class UploadFilesRequest(IReadOnlyList<IBrowserFile> files, string type, string identifier)
{
    public required IReadOnlyList<IBrowserFile> Files { get; set; } = files;
    public required string Identifier { get; set; } = identifier;
    public required string Type { get; set; } = type;
}