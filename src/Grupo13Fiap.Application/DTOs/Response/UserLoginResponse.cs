using Grupo13Fiap.Utils.Extensions;

using System.Text.Json.Serialization;

namespace Grupo13Fiap.Application.DTOs.Response
{
    public class UserLoginResponse
    {
        public bool Success => Errors.Count.IsZero();

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AccessToken { get; private set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RefreshToken { get; private set; }

        public List<string> Errors { get; private set; }

        public UserLoginResponse() =>
            Errors = [];

        public UserLoginResponse(bool success, string accessToken, string refreshToken) : this()
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public void AddError(string error) =>
            Errors.Add(error);

        public void AddErrors(IEnumerable<string> errors) =>
            Errors.AddRange(errors);
    }
}