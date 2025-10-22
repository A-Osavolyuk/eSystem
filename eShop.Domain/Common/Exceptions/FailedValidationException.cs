namespace eShop.Domain.Common.Exceptions;

public class FailedValidationException : Exception
{
    public FailedValidationException(IEnumerable<ValidationFailure> errors, string errorMessage = "Validation error(s)") : base(errorMessage)
    {
        Errors = errors.Select(x => x.ErrorMessage);
    }

    public IEnumerable<string> Errors { get; }
};