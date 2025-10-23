namespace eAccount.Blazor.Server.Domain.Models.Products;

public class FruitPropertiesModel : ProductPropertiesModel
{
    public string Color { get; set; } = string.Empty;
    public string Variety { get; set; } = string.Empty;
    public string CountryOfOrigin { get; set; } = string.Empty;
    public string RipenessStage { get; set; } = string.Empty;
    public string StorageTemperature { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public bool RequiresRefrigeration { get; set; }
    public bool ContainsSeeds { get; set; }
    public bool IsTropical { get; set; }
    public bool IsOrganic { get; set; }
}