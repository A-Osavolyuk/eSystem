namespace eShop.Domain.Common.API;

public record FileRequest(
    FileData Data,
    HttpMethods Methods,
    string Url);

public class FileData(IReadOnlyList<IBrowserFile> files)
{
    public IReadOnlyList<IBrowserFile> Files { get; init; } = files;
}