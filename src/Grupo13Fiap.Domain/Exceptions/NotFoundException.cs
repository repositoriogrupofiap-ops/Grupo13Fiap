namespace Grupo13Fiap.Domain.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string resource, object key)
        : base($"'{resource}' with key '{key}' was not found.") { }

    public NotFoundException(string message) : base(message) { }
}
