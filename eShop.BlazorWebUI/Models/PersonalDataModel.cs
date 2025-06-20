using eShop.Domain.Enums;

namespace eShop.BlazorWebUI.Models;

public class PersonalDataModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}