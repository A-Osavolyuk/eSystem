namespace eShop.Domain.Common.Api;

public record FileRequest(
    FileData Data,
    HttpMethods Methods,
    string Url);

public class FileData
{
    public IBrowserFile File { get; init; }
    public IReadOnlyList<IBrowserFile> Files { get; init; }

    public FileData(IReadOnlyList<IBrowserFile> files)
    {
        Files = files;
        File = null!;
    }

    public FileData(IBrowserFile file)
    {
        File = file;
        File = null!;
    }
}