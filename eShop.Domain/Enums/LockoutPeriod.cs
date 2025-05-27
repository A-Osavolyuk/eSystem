namespace eShop.Domain.Enums;

public enum LockoutPeriod
{
    None,
    Day,
    Week,
    Month,
    Quarter,
    Year,
    Permanent,
    Custom,
}