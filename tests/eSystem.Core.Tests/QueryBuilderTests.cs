using eSystem.Core.Utilities.Query;

namespace eSystem.Core.Tests;

public class QueryBuilderTests
{
    public static TheoryData<string, Dictionary<string, string>, string> QueryParserTestData =>
        new()
        {
            { "", [], "" },
            {
                "https://example.com",
                new Dictionary<string, string>(),
                "https://example.com"
            },
            {
                "https://example.com",
                new Dictionary<string, string> { { "key", "some" } },
                "https://example.com?key=some"
            },
            {
                "https://example.com",
                new Dictionary<string, string> { { "key", "some" }, { "token", "1234" } },
                "https://example.com?key=some&token=1234"
            },
            {
                "https://example.com?key=some",
                new Dictionary<string, string> { { "token", "1234" } },
                "https://example.com?key=some&token=1234"
            },
            {
                "https://example.com",
                new Dictionary<string, string> { { "key", "a b&c" } },
                "https://example.com?key=a+b%26c"
            }
        };

    [Theory]
    [MemberData(nameof(QueryParserTestData))]
    public void Build_ShouldReturnExpectedUri(string uri, Dictionary<string, string> queryParams, string expected)
    {
        //Arrange
        var builder = QueryBuilder.Create().WithUri(uri);
        foreach (var param in queryParams)
        {
            builder.WithQueryParam(param.Key, param.Value);
        } 

        //Act
        var result = builder.Build();

        //Assert
        Assert.Equal(expected, result);
    }
}