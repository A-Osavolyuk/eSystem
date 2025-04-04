using eShop.Domain.Abstraction.Exceptions;

namespace eShop.Domain.Exceptions;

public class NotFoundException(string message) : Exception(message), INotFoundException;