using eSystem.Core.Utilities.Query;

namespace eSystem.Core.Tests;

public class QueryParserTests
{
    [Fact]
    public void GetQueryParams_WhenUriNotContainsParams_ShouldReturnEmptyDictionary()
    {
        //Arrange
        const string uri = "https://example.com";
        
        //Act
        var queryParams = QueryParser.GetQueryParameters(uri);

        //Assert
        Assert.Empty(queryParams);
    }
    
    [Fact]
    public void GetQueryParams_WhenUriContainsParams_ShouldReturnDictionaryWithParams()
    {
        //Arrange
        const string uri = "https://example.com?key=some&token=1234";
        
        //Act
        var queryParams = QueryParser.GetQueryParameters(uri);

        //Assert
        Assert.NotEmpty(queryParams);
        Assert.Contains("key", queryParams);
        Assert.Equal("some", queryParams["key"]);
        Assert.Contains("token", queryParams);
        Assert.Equal("1234", queryParams["token"]);
    }
}