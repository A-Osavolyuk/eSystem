using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eShop.Product.Api.Types;

public class Price
{
    [BsonRepresentation(BsonType.String)] public Currency Currency { get; set; }
    public decimal Amount { get; set; }
}