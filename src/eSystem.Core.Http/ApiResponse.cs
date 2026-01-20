using System.Text.Json;
using eSystem.Core.Http.Results;

namespace eSystem.Core.Http;

public sealed class ApiResponse
{
    public bool Succeeded { get; private init; }
    private string? Result { get; init; }
    private Error Error { get; init; } = null!;
    
    public static ApiResponse Success(string? result = null) => new(){ Succeeded = true, Result = result };
    public static ApiResponse Fail(Error error) => new(){ Succeeded = false, Error = error };
    
    public Error GetError() => Error;

    public bool TryGetValue<TValue>(out TValue? value)
    {
        if (string.IsNullOrWhiteSpace(Result))
        {
            value = default;
            return false;
        }

        try
        {
            value = JsonSerializer.Deserialize<TValue>(Result, new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return true;
        }
        catch (JsonException)
        {
            value = default;
            return false;
        }
    }
}