using eSystem.Core.Http;
using eSystem.Core.Http.Results;

namespace eSystem.Core.Http.Tests;

public class ApiResponseTests
{
    [Fact]
    public void TryGetValue_WhenResultIsNull_ShouldReturnFalseAndDefaultValue()
    {
        //Arrange
        var response = ApiResponse.Success();

        //Act
        var result = response.TryGetValue<bool>(out var value);

        //Assert
        Assert.False(result);
        Assert.False(value);
    }
    
    [Fact]
    public void TryGetValue_WhenResultIsInvalidJson_ShouldReturnFalseAndDefaultValue()
    {
        //Arrange
        var response = ApiResponse.Success("{");

        //Act
        var result = response.TryGetValue<string>(out var value);

        //Assert
        Assert.False(result);
        Assert.Null(value);
    }
    
    [Fact]
    public void TryGetValue_WhenResultIsValidJsonButTypeIsIncompatible_ShouldReturnFalseAndDefaultValue()
    {
        //Arrange
        var response = ApiResponse.Success("\"string\"");

        //Act
        var result = response.TryGetValue<int>(out var value);

        //Assert
        Assert.False(result);
        Assert.Equal(0, value);
    }
    
    [Fact]
    public void TryGetValue_WhenResultIsValidAndTypeIsCompatible_ShouldReturnTrueAndString()
    {
        //Arrange
        var response = ApiResponse.Success("\"string\"");

        //Act
        var result = response.TryGetValue<string>(out var value);

        //Assert
        Assert.True(result);
        Assert.Equal("string", value);
    }
    
    [Fact]
    public void GetError_ShouldReturnError()
    {
        //Arrange
        var error = new Error { Code = "error", Description = "description" };
        var response = ApiResponse.Fail(error);
        
        //Act
        var resultError = response.GetError();
        
        //Assert
        Assert.False(response.Succeeded);
        Assert.Same(resultError, error);
    }
}