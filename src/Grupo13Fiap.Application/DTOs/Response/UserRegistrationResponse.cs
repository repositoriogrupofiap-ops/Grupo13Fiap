namespace Grupo13Fiap.Application.DTOs.Response
{
    public class UserRegistrationResponse
    {
        public bool Success { get; private set; }
        public List<string> Erros { get; private set; }

        public UserRegistrationResponse() =>
            Erros = new List<string>();

        public UserRegistrationResponse(bool success = true) : this() =>
            Success = success;

        public void AddErrors(IEnumerable<string> errors) =>
            Erros.AddRange(errors);
    }
}