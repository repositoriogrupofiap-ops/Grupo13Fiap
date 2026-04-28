namespace Grupo13Fiap.Application.DTOs.Response;

public class UserRegistrationResponse
{
    public bool         Success        { get; }
    public string?      IdentityUserId { get; }
    public List<string> Erros          { get; } = [];

    public UserRegistrationResponse(bool success, string? identityUserId = null)
    {
        Success        = success;
        IdentityUserId = identityUserId;
    }

    public void AddErrors(IEnumerable<string> errors) => Erros.AddRange(errors);
}