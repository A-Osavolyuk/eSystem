namespace eShop.Domain.Common.API;

public class FileRequest
{
    public required FileData Data { get; set; }
    public required HttpMethod Method { get; set; }
    public required string Url { get; set; }
    public required Metadata Metadata { get; set; }
}

public class FileData(IReadOnlyList<IBrowserFile> files)
{
    public IReadOnlyList<IBrowserFile> Files { get; init; } = files;
}

public class Metadata
{
    public string Identifier { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}