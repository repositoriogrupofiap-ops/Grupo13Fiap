namespace Grupo13Fiap.Domain.Exceptions;

public class ValidationException : DomainException
{
    public IReadOnlyCollection<string> Errors { get; }

    public ValidationException(IEnumerable<string> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors.ToList().AsReadOnly();
    }

    public ValidationException(string message) : base(message)
    {
        Errors = new List<string> { message }.AsReadOnly();
    }
}
