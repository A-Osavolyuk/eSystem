namespace eShop.Domain.Requests.API.Storage;

public class LoadFilesRequest
{
    public List<string> Identifiers { get; set; } = [];
}