using eSystem.Core.Utilities.Query;

namespace eSystem.Core.Tests;

public class QueryBuilderTests
{
    [Fact]
    public void Build_WithEmptyUriAndNoQueryParams_ShouldReturnEmptyString()
    {
        //Arrange
        var builder = QueryBuilder.Create().WithUri("");

        //Act
        var result = builder.Build();

        //Assert
        Assert.Equal("", result);
    }
    
    [Fact]
    public void Build_WithUriAndNoQueryParams_ShouldReturnUri()
    {
        //Arrange
        var builder = QueryBuilder.Create().WithUri("https://example.com");

        //Act
        var result = builder.Build();

        //Assert
        Assert.Equal("https://example.com", result);
    }
    
    [Fact]
    public void Build_WithSingleQueryParam_ShouldReturnCorrectUri()
    {
        //Arrange
        var builder = QueryBuilder.Create()
            .WithUri("https://example.com")
            .WithQueryParam("key", "some");

        //Act
        var result = builder.Build();

        //Assert
        Assert.Equal("https://example.com?key=some", result);
    }
    
    [Fact]
    public void Build_WithMultipleQueryParams_ShouldReturnCorrectUri()
    {
        //Arrange
        var builder = QueryBuilder.Create()
            .WithUri("https://example.com")
            .WithQueryParam("key", "some")
            .WithQueryParam("token", "1234");

        //Act
        var result = builder.Build();

        //Assert
        Assert.Equal("https://example.com?key=some&token=1234", result);
    }
    
    [Fact]
    public void Build_WithExistingQuery_ShouldUseAmpersand()
    {
        //Arrange
        var builder = QueryBuilder.Create()
            .WithUri("https://example.com?key=some")
            .WithQueryParam("token", "1234");

        //Act
        var result = builder.Build();

        //Assert
        Assert.Equal("https://example.com?key=some&token=1234", result);
    }
    
    [Fact]
    public void Build_WithSpecialCharacters_ShouldUrlEncodeValues()
    {
        //Arrange
        var builder = QueryBuilder.Create()
            .WithUri("https://example.com")
            .WithQueryParam("key", "a b&c");

        //Act
        var result = builder.Build();

        //Assert
        Assert.Equal("https://example.com?key=a+b%26c", result);
    }
}