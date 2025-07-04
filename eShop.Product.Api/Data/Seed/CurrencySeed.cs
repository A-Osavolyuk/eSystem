namespace eShop.Product.Api.Data.Seed;

public class CurrencySeed : Seed<CurrencyEntity>
{
    public override List<CurrencyEntity> Get()
    {
        return
        [
            new CurrencyEntity
            {
                Id = Guid.Parse("91027200-4af5-4386-b966-a86c1bb3b097"),
                Name = "US Dollar",
                Code = "USD",
                Symbol = "$"
            },
            new CurrencyEntity
            {
                Id = Guid.Parse("9dc820e1-fd82-480b-b7ba-8a0f5b57e10b"),
                Name = "Euro",
                Code = "EUR",
                Symbol = "€"
            },
            new CurrencyEntity
            {
                Id = Guid.Parse("edd5cdfa-e6f7-479e-88d7-37ecd6a09bbd"),
                Name = "British Pound",
                Code = "GBP",
                Symbol = "£"
            },
            new CurrencyEntity
            {
                Id = Guid.Parse("8d9de2a5-7694-4916-9096-3a2e0c960d5c"),
                Name = "Japanese Yen",
                Code = "JPY",
                Symbol = "¥"
            },
            new CurrencyEntity
            {
                Id = Guid.Parse("5ea4d82b-c1b3-4744-83b0-24d3d764e778"),
                Name = "Swiss Franc",
                Code = "CHF",
                Symbol = "Fr"
            },
            new CurrencyEntity
            {
                Id = Guid.Parse("cc8d1e9a-6a4b-41bf-8d07-0e1f9d274707"),
                Name = "Ukrainian Hryvnia",
                Code = "UAH",
                Symbol = "₴"
            },
            new CurrencyEntity
            {
                Id = Guid.Parse("1c65450b-3100-440d-b2bc-16151f78c597"),
                Name = "Canadian Dollar",
                Code = "CAD",
                Symbol = "$"
            },
            new CurrencyEntity
            {
                Id = Guid.Parse("f7660ea2-f004-4d14-ac54-a865fa251aac"),
                Name = "Australian Dollar",
                Code = "AUD",
                Symbol = "$"
            },
            new CurrencyEntity
            {
                Id = Guid.Parse("1a7c6f15-4ea6-4519-9ed4-d476a725ae91"),
                Name = "Chinese Yuan",
                Code = "CNY",
                Symbol = "¥"
            }
        ];
    }
}