using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace Grupo13Fiap.WebApi.Controllers.Shared
{
    public class CustomProblemDetails : ProblemDetails
    {
        public List<string> Errors { get; private set; }

        public CustomProblemDetails(HttpStatusCode status, string? detail = null, IEnumerable<string>? errors = null) : this()
        {
            Title = status switch
            {
                HttpStatusCode.BadRequest => "One or more validation errors occurred.",
                HttpStatusCode.Unauthorized => "Unauthorized.",
                HttpStatusCode.Forbidden => "Access denied.",
                HttpStatusCode.NotFound => "Resource not found.",
                HttpStatusCode.Conflict => "Conflict.",
                HttpStatusCode.NotImplemented => "Not implemented.",
                HttpStatusCode.InternalServerError => "Internal server error.",
                _ => "An error has occurred."
            };

            Status = (int)status;
            Detail = detail;

            if(errors is not null)
            {
                Errors.AddRange(errors);

                Detail = Errors.Count == 1
                    ? Errors[0]
                    : Errors.Count > 1
                        ? "Multiple problems have occurred."
                        : detail;
            }
        }

        public CustomProblemDetails(HttpStatusCode status, HttpRequest request, string? detail = null, IEnumerable<string>? errors = null) : this(status, detail, errors) =>
            Instance = request.Path;

        private CustomProblemDetails() =>
            Errors = [];
    }
}
