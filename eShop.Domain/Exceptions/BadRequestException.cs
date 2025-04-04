using eShop.Domain.Abstraction.Exceptions;

namespace eShop.Domain.Exceptions;

public class BadRequestException(string message) : Exception(message), IBadRequestException;