namespace eShop.Domain.Common.API;

public class FileRequest
{
    public required FileData Data { get; set; }
    public required HttpMethod Method { get; set; }
    public required string Url { get; set; }
}

public class FileData(IReadOnlyList<IBrowserFile> files)
{
    public IReadOnlyList<IBrowserFile> Files { get; init; } = files;
}