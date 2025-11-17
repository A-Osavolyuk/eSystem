namespace eSystem.Core.Common.Results;

public class Error
{
    public required string ErrorCode { get; init; }
    public required string ErrorDescription { get; init; }
}