namespace eShop.BlazorWebUI.Models.Products;

public class VegetablePropertiesModel : ProductPropertiesModel
{
    public string Color { get; set; } = string.Empty;
    public string Variety { get; set; } = string.Empty;
    public string CountryOfOrigin { get; set; } = string.Empty;
    public string RipenessStage { get; set; } = string.Empty;
    public string StorageTemperature { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public bool RequiresRefrigeration { get; set; }
    public bool IsLeafy { get; set; }
    public bool IsRootVegetable { get; set; }
    public bool IsOrganic { get; set; }
}