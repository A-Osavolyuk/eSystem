namespace eSystem.Core.Common.Results;

public enum ErrorCode
{
    None = 0,
    SeeOther = 303,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    InternalServerError = 500,
}