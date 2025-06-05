namespace eShop.Domain.Requests.API.Storage;

public class LoadFilesRequest
{
    public string ResourceGroup { get; set; } = string.Empty;
    public List<string> Identifiers { get; set; } = [];
}